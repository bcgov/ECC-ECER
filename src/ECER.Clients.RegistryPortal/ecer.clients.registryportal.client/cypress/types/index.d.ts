declare global {
  namespace Cypress {
    interface Chainable<Subject = any> {
      /**
       * Custom command to reset the state by clearing cookies, local storage, and session storage.
       * @example cy.resetState()
       */
      resetState(): Chainable<null>;

      /**
       * Custom command to log in using default credentials.
       * @example cy.login()
       */
      login(username?: string, password?: string): Chainable<any>;
    }
  }
}

export {};
