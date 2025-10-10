export interface DocItem {
  id: string;
  title: string;
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
