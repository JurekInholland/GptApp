import ApiService from "@/api";
import type {
  IAPIResponse,
  IModelSettings,
  ITokenCount,
  ITokenResponse,
} from "@/types";
import { prompts } from "@/constants";
export const apiService = new ApiService();

export const getTotalTokens = (tokenCount: ITokenCount): number => {
  return (
    tokenCount.historyTokens +
    tokenCount.queryTokens +
    tokenCount.systemPromptTokens
  );
};

export const getFormattedTokenCount = (response: IAPIResponse): string => {
  const tokenCount = response.tokenCount;
  const tokenKind = response.role === "bot" ? "completion" : "query";
  let result = `${tokenCount.queryTokens} ${tokenKind}`;
  if (tokenCount.historyTokens > 0) {
    result = `${tokenCount.historyTokens} history + ${result}`;
  }
  if (tokenCount.systemPromptTokens > 0) {
    result = `${tokenCount.systemPromptTokens} system + ${result}`;
  }
  return result;
};

export const getTemperatureString = (temperature: number): string => {
  if (temperature <= 0.1) {
    return "non random";
  } else if (temperature <= 0.5) {
    return "slightly random";
  } else if (temperature <= 1) {
    return "random";
  } else if (temperature <= 1.5) {
    return "very random";
  }
  return "extremely random";
};

export const countTokens = async (text: string): Promise<number> => {
  if (text.length === 0) return 0;
  const requestTokens: ITokenResponse = (await apiService.GetTokens(
    `${text}`
  )) as ITokenResponse;
  return requestTokens.tokenCount;
};

export const getSystemPromptTokens = (modelSettings: IModelSettings) => {
  if (modelSettings.systemPrompt === "custom")
    return modelSettings.customPrompt;
  return prompts[modelSettings.systemPrompt].prompt;
};
