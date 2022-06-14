import { IEmbedReport } from "./embed-report.model";

export interface IEmbedParams {
  type: string;
  report: IEmbedReport;
  token: IEmbedToken;
  

}

export interface IEmbedToken {
  expiration: Date;
  token: string;
  tokenId: string;
}
