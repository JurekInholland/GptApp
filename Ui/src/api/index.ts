import type { IChatCompletionRequest, ITokenResponse } from "@/types";
import axios, {
  AxiosError,
  type AxiosInstance,
  type AxiosRequestConfig,
} from "axios";

export default class ApiService {
  private axiosInstance: AxiosInstance;
  constructor() {
    this.axiosInstance = axios.create({
      baseURL: "/api",
    });
  }
  private async axiosCall<T>(
    config: AxiosRequestConfig
  ): Promise<AxiosError | T> {
    try {
      config.headers = { "Content-Type": "application/json" };
      const { data } = await this.axiosInstance.request<T>(config);
      return data;
    } catch (error) {
      return error as AxiosError;
    }
  }
  public async GetTokens(
    text: string
  ): Promise<AxiosError | ITokenResponse> {
    return await this.axiosCall({
      method: "post",
      url: "/Token/GetTokenCount",
      data: text,
    });
  }

  public async getChatCompletions(
    request: IChatCompletionRequest,
    connectionId: string
  ) {
    // console.log("getChatCompletions", request, connectionId);
    return this.axiosCall({
      method: "post",
      url: "/Chat/ServiceRequest",
      data: request,
      params: { connectionId: connectionId },
    });
  }
}
