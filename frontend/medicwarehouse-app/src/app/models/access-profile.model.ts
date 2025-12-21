export interface AccessProfile {
  id: string;
  name: string;
  description: string;
  isDefault: boolean;
  isActive: boolean;
  clinicId?: string;
  clinicName?: string;
  createdAt: Date;
  updatedAt?: Date;
  permissions: string[];
  userCount: number;
}

export interface CreateAccessProfile {
  name: string;
  description: string;
  clinicId: string;
  permissions: string[];
}

export interface UpdateAccessProfile {
  name: string;
  description: string;
  permissions: string[];
}

export interface Permission {
  key: string;
  description: string;
}

export interface PermissionCategory {
  category: string;
  permissions: Permission[];
}

export interface AssignProfile {
  userId: string;
  profileId: string;
}
