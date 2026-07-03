import type { List } from './list';

export interface Board {
  id: string;
  name: string;
  description: string | null;
  backgroundColor: string | null;
  backgroundImgUrl: string | null;
  visibility: string;
  isArchived: boolean;
  archivedAt: string | null;
  createdAt: string;
  updatedAt: string;
  lists?: List[];
}
