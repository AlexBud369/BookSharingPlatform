export interface BookCreateDto {
  title: string;
  author: string;
  description?: string;
  tags: string[]; 
  coverImage?: FileList;
}

export interface BookDto {
  id: string; 
  title: string;
  author: string;
  description?: string;
  coverImageUrl?: string; 
  createdByUserId: string;
  createdAt: string;
  tags: TagDto[];
}

export interface BookFilterDto {
  searchQuery?: string;
  tagIds?: string[];
  createdByUserId?: string;
  sortBy?: 'title' | 'createdAt';
  sortDescending: boolean;
  pageNumber: number;
  pageSize: number;
}

export interface BookUpdateDto {
  title?: string;
  author?: string;
  description?: string;
  coverImage?:FileList;
  tags?: string[];
}

export interface TagCreateDto {
  tagName: string;
}

export interface TagDto {
  id: string;
  tagName: string;
  bookIds: string[];
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  role: string;
  isBlocked: boolean;
  createdAt: string;
  bookIds: string[];
}

export interface UserUpdateDto {
  username?: string;
  email?: string;
  password?: string;
}

export interface ChangeRoleRequestDto {
  role: string;
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  user: UserDto;
}

export interface PagedResponseDto<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface RefreshTokenDto {
  id: string;
  token: string;
  userId: string;
  createdAt: string;
  expiresAt: string;
  isRevoked: boolean;
}