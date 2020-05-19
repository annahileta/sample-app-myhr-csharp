export interface IUser {
  Id: number;
  Name: string;
  //date: string;
  Email: string;
  //imageUrl: string;
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
