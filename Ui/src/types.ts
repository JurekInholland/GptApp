export interface IWebsocketMessage {
  id: string;
  content: string;
  finishReason: string;
}

export interface IAPIResponse {
  message: string;
  status: string;
  date: string;
  role: string;
  included: boolean;
  tokenCount: ITokenCount;
}
export interface ITokenResponse {
  characterCount: number;
  tokenCount: number;
  tokens: number[];
}
export interface ITokenCount {
  systemPromptTokens: number;
  queryTokens: number;
  historyTokens: number;
}

export interface IModelSettings {
  model: string;
  systemPrompt: string;
  includeHistory: boolean;
  temperature: string;
  maxTokens: string;
  presencePenalty: number;
  frequencyPenalty: number;
  customPrompt: string;
}
export interface ICompletionMessage {
  role: string;
  content: string;
  name?: string;
}

export interface IPrompt {
  name: string;
  slug: string;
  prompt: string;
  description: string;
}

/**
 * This is the request body for the OpenAI API Proxy
 */
export interface IChatCompletionRequest {
  model: string;
  messages: ICompletionMessage[];
  stream: boolean;
  n: number;
  max_tokens: number;
  temperature: number;
  presence_penalty: number;
  frequency_penalty: number;
}

export enum ImageSize {
  Small = "256x256",
  Medium = "512x512",
  Large = "1024x1024",
}

/**
 * Request body for the image generation proxy endpoint
 */
export interface IImageRequest {
  prompt: string;
  size: ImageSize;
}
