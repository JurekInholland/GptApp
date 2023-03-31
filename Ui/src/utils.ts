import type { IAPIResponse, ITokenCount } from "./types";

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
