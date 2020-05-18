export interface IUser {
  id: number;
  name: string;
  date: string;
  email: string;
  imageUrl: string;
  address?: {
    address1: string;
    address2: string;
    city: string;
    country: string;
    fax: string;
    phone: string;
    postalCode: string;
    stateOrProvince: string;
  };
  isAdmin: boolean;
}
