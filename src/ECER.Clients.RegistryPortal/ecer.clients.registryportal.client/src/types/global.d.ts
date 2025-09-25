declare global {
  interface Window {
    turnstile: {
      render(id: String, options: TurnstileOptions): string;
      reset(string): void;
    };
  }
}

interface TurnstileOptions {
  sitekey: string | undefined | null;
  callback?: (token: string) => void;
  "expired-callback"?: () => void;
  "timeout-callback"?: () => void;
  theme?: "light" | "dark" | "auto";
  "response-field"?: boolean;
  "response-field-name"?: string;
  size?: "normal" | "compact" | "invisible" | "flexible";
}

export {};
