import type { Card } from './card';

export interface List {
  id: string;
  boardId: string;
  name: string;
  position: number;
  isArchived: boolean;
  archivedAt: string | null;
  createdAt: string;
  updatedAt: string;
  cards?: Card[];
}
