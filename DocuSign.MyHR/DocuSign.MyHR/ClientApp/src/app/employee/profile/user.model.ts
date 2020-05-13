export interface IUser {
  id: number;
  name: string;
  date: string;
  imageUrl: string;
  location?: {
    address: string;
    city: string;
    country: string;
  };
  isAdmin: boolean;
}
