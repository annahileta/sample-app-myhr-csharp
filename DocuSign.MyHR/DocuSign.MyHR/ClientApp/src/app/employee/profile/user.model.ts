export interface IUser {
  id: number;
  name: string;
  firstName: string,
  lastName: string, 
  date: string;
  email: string;
  profileImageUrl: string;
  profileId:string,
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
