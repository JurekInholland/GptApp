interface IWebsocketMessage {
  id: string;
  content: string;
  finishReason: string;
}

interface IAPIResponse {
  message: string;
  status: string;
  date: string;
  role: string;
}

interface IModelSettings {
  model: string;
  systemPrompt: string;
  includeHistory: boolean;
  temperature: number;
  maxTokens: number;
  presencePenalty: number;
  frequencyPenalty: number;
}
export interface ICompletionMessage {
  role: string;
  content: string;
}

export interface IPrompt {
  name: string;
  slug: string;
  prompt: string;
  description: string;
}

export interface ICompletionRequest {
  model: string;
  messages: ICompletionMessage[];
  stream: boolean;
  n: number;
  max_tokens: number;
  temperature: number;
  presence_penalty: number;
  frequency_penalty: number;
}

export const defaultSettings: IModelSettings = {
  model: "gpt-3.5-turbo",
  systemPrompt: "default",
  includeHistory: true,
  temperature: 0.7,
  maxTokens: 100,
  frequencyPenalty: 0.0,
  presencePenalty: 0.0,
} as IModelSettings;

export type { IWebsocketMessage, IAPIResponse, IModelSettings };
