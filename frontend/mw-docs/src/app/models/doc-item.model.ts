export interface DocItem {
  id: string;
  title: string;
  fullTitle?: string;  // Full title extracted from markdown
  category: string;
  path: string;
  description: string;
  size?: string;
  idealFor?: string;
}

export interface DocCategory {
  name: string;
  icon: string;
  docs: DocItem[];
}
