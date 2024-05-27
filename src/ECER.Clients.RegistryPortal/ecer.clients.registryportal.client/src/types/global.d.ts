declare global {
  interface Window {
    grecaptcha: {
      getResponse(): string;
      reset(): void;
    };
    recaptchaSuccessCallback: function;
    recaptchaExpiredCallback: function;
  }
}

export {};
