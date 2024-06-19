declare global {
  interface Window {
    grecaptcha: {
      getResponse(): string;
      reset(string): void;
      render(string): string;
    };
    recaptchaSuccessCallback: function;
    recaptchaExpiredCallback: function;
    recaptchaOnloadCallback: function;
  }
}

export {};
