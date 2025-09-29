declare global {
  interface globalThis {
    grecaptcha: {
      getResponse(): string;
      reset(string?): void; //if widget id is not provided, will default to first recaptcha instance
      render(string): string;
    };
    recaptchaSuccessCallback: function;
    recaptchaExpiredCallback: function;
    recaptchaOnloadCallback: function;
  }
}

export {};
