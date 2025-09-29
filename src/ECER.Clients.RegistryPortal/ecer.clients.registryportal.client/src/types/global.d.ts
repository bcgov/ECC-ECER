declare global {
  var grecaptcha: {
    getResponse(): string;
    reset(widgetId?: string | number): void;
    render(container: string): string;
  };

  var recaptchaSuccessCallback: function;
  var recaptchaExpiredCallback: function;
  var recaptchaOnloadCallback: function;
}

export {};
