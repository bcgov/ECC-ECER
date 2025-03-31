declare global {
  namespace Cypress {
    interface Chainable<Subject = any> {
      /**
       * Custom command to reset the User state by deleting all applications and certificates
       * @example cy.resetUserState()
       */
      resetUserState(): Chainable<Response<any>>;
    }
  }
}

export {};
