---
name: angular-cdk
description: Use this skill whenever working on Angular UI in this Trello-clone project that involves drag-and-drop (moving Cards between Lists, reordering Lists on a Board), floating/overlay UI (Card detail modal, dropdown menus, tooltips, context menus), keyboard/focus accessibility for modals, responsive breakpoints, or virtualized/long scrolling lists. Trigger this any time the user mentions "drag", "drop", "reorder", "sortable", "modal", "overlay", "dropdown", "popover", "focus trap", "responsive", "breakpoint", or "virtual scroll" in the context of the Angular frontend, even if they don't say "CDK" explicitly. Always check this skill before writing custom drag/drop or overlay-positioning logic by hand — Angular CDK already solves it.
---

# Angular CDK for the Trello Clone

Angular CDK (`@angular/cdk`) provides unstyled, accessible interaction primitives. Material is built on top of it. Always prefer CDK primitives over hand-rolled drag/drop, positioning, or focus-trap logic — they handle browser quirks, touch devices, and a11y that are easy to get wrong.

**Version:** this project targets Angular 22, so install `@angular/cdk@^22.0.0` (major version must match Angular's major version).

```bash
ng add @angular/cdk
```

Each CDK feature is a separate standalone import — only import what you use.

---

## 1. Drag & Drop — Board / List / Card (the core use case)

Module: `@angular/cdk/drag-drop` → `CdkDropList`, `CdkDrag`, `CdkDropListGroup`, `moveItemInArray`, `transferArrayItem`.

### Two nested drag scopes in this app

The Board has **two independent drag interactions** that must not be confused:

1. **Reordering Lists** within a Board (horizontal drag of whole columns).
2. **Moving/reordering Cards** between Lists (vertical drag inside a column, or across columns).

Use `cdkDropListGroup` on the Board container so any Card-level `cdkDropList` inside it can auto-connect, instead of manually listing every list ID in `cdkDropListConnectedTo`.

```typescript
import { CdkDropListGroup, CdkDropList, CdkDrag, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board',
  imports: [CdkDropListGroup, CdkDropList, CdkDrag],
  template: `
    <div cdkDropListGroup class="board">
      <!-- List reordering (horizontal) -->
      <div cdkDropList
           cdkDropListOrientation="horizontal"
           [cdkDropListData]="lists()"
           (cdkDropListDropped)="onListDrop($event)"
           class="lists-row">
        @for (list of lists(); track list.id) {
          <div cdkDrag class="list-column">
            <div class="list-header" cdkDragHandle>{{ list.name }}</div>

            <!-- Card reordering / cross-list moves (vertical) -->
            <div cdkDropList
                 [id]="list.id"
                 [cdkDropListData]="list.cards"
                 (cdkDropListDropped)="onCardDrop($event)"
                 class="card-list">
              @for (card of list.cards; track card.id) {
                <div cdkDrag class="card">{{ card.title }}</div>
              }
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class BoardComponent {
  lists = signal<List[]>([]);

  onListDrop(event: CdkDragDrop<List[]>) {
    moveItemInArray(this.lists(), event.previousIndex, event.currentIndex);
    // PATCH /lists/reorder — send new Position values for affected Lists
  }

  onCardDrop(event: CdkDragDrop<Card[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      // PATCH the moved card's Position within the same List
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      // PATCH the moved card's ListId AND Position — both changed
    }
  }
}
```

### Rules specific to this project

- **`cdkDragHandle`**: put it on the List header only, so dragging a List doesn't fire when the user is scrolling/clicking inside a Card. Cards themselves stay fully draggable (no handle needed) since the whole card body is the "grab" target.
- **Always persist Position as a float or gap-based integer** (e.g. `1000, 2000, 3000` steps), not a plain sequential index. Re-indexing every sibling row on every drop is expensive and causes write contention; a gap-based position lets you insert between two cards by averaging (`(1000+2000)/2 = 1500`) without touching other rows. Only re-normalize positions in a background job if gaps get too small.
- **Cross-list drop = two field changes**, not one: `ListId` (new parent) and `Position` (new order). Never update just one — that reproduces the "duplicate List" bug class from the ERD (data that looks consistent in the UI but is ambiguous in the DB).
- **Optimistic UI**: `moveItemInArray`/`transferArrayItem` mutate local state immediately. Send the PATCH after, and roll back the local array (or refetch) if the request fails — don't block the drag animation on the network call.
- Use `cdkDragBoundary` on the drop-list container if Cards or Lists should not be draggable outside the Board viewport.
- For very long card lists, combine with `@angular/cdk/scrolling` (`cdk-virtual-scroll-viewport`) instead of rendering hundreds of Card DOM nodes — see §3.

---

## 2. Overlay & Portal — Card Detail Modal, Dropdown Menus

Module: `@angular/cdk/overlay` (+ `@angular/cdk/portal` for injecting component content).

Use this for:
- **Card detail modal** (click a Card → full editor with Comments, Checklist, Attachments, Labels).
- **Label picker / member picker dropdowns** on a Card.
- **Context menus** ("..." menu on a Card or List).

```typescript
import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';

constructor(private overlay: Overlay) {}

openCardDetail(card: Card) {
  const overlayRef = this.overlay.create({
    hasBackdrop: true,
    backdropClass: 'cdk-overlay-dark-backdrop',
    positionStrategy: this.overlay.position().global().centerHorizontally().centerVertically(),
    scrollStrategy: this.overlay.scrollStrategies.block(),
  });
  const portal = new ComponentPortal(CardDetailComponent);
  const componentRef = overlayRef.attach(portal);
  componentRef.instance.card = card;
  overlayRef.backdropClick().subscribe(() => overlayRef.dispose());
}
```

For anchored UI (dropdown attached to a button, not centered), use `overlay.position().flexibleConnectedTo(elementRef)` instead of `global()`.

**Don't hand-roll `position: fixed` + z-index stacks for modals/dropdowns** — the Overlay's `FlexibleConnectedPositionStrategy` already handles viewport-edge flipping (e.g. a Label dropdown near the bottom of the screen opens upward automatically).

---

## 3. Scrolling — Long Card Lists / Long Boards

Module: `@angular/cdk/scrolling` → `CdkVirtualScrollViewport`, `cdkVirtualFor`.

If a List can hold hundreds of Cards (Trello lists often do), don't render them all. Wrap the card list in a virtual scroll viewport once a List crosses a practical threshold (~50+ cards):

```html
<cdk-virtual-scroll-viewport itemSize="72" class="card-list">
  <div *cdkVirtualFor="let card of list.cards" cdkDrag class="card">{{ card.title }}</div>
</cdk-virtual-scroll-viewport>
```

Note: mixing virtual scroll with drag-drop needs care — CDK drag-drop and virtual-scroll do interoperate, but only render what's likely to be dragged into view; test cross-list drops when the source list is virtualized.

---

## 4. Accessibility — Focus Trapping

Module: `@angular/cdk/a11y` → `CdkTrapFocus`, `LiveAnnouncer`.

Add `cdkTrapFocus cdkTrapFocusAutoCapture` inside the Card Detail modal so Tab/Shift+Tab cycles within the modal instead of leaking focus to the Board behind it. Use `LiveAnnouncer` to announce drag-drop outcomes for screen reader users (e.g. "Card moved to Done list, position 3") — CDK drag-drop does not announce this automatically.

---

## 5. Layout — Responsive Board

Module: `@angular/cdk/layout` → `BreakpointObserver`.

```typescript
constructor(private breakpointObserver: BreakpointObserver) {}

isMobile = toSignal(
  this.breakpointObserver.observe('(max-width: 768px)').pipe(map(r => r.matches))
);
```

Use this to switch the Board from horizontal multi-list layout to a single-list-at-a-time swipeable layout on mobile, rather than duplicating breakpoint logic with raw CSS media queries in every component.

---

## Common pitfalls in this codebase

- Forgetting `cdkDropListGroup` and instead manually maintaining a `connectedTo` array of every List ID — works but breaks silently when a List is added/removed without updating the array. Prefer the group.
- Updating only `Position` on a cross-list Card move and forgetting `ListId` (or vice versa) — always update both together in one PATCH to avoid an inconsistent intermediate state.
- Building a custom modal with `*ngIf` + fixed positioning instead of `cdk/overlay` — loses backdrop click-to-close, scroll blocking, and viewport-aware positioning for free.
- Not adding `cdkDragHandle` to List headers — makes it too easy to accidentally start dragging a whole List while trying to interact with a Card inside it.
-