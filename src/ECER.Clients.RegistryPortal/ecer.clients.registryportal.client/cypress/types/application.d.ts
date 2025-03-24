declare global {
  namespace Cypress {
    interface Chainable<Subject = any> {
      /**
       * Custom command to delete an application by its id.
       * @example cy.resetApplicationState('application-id')
       */
      resetApplicationState(applicationId: string): Chainable<Response<any>>;

      /**
       * Custom command to register API intercepts for application workflow.
       * This command sets up intercepts that update a global variable with the latest response
       * @example
       * cy.registerApiIntercepts();
       */
      registerApiIntercepts(): Chainable<any>;

      /**
       * Custom command to retrieve the current application ID.
       * This command returns the application ID as a string from the internal application state,
       * @example
       * cy.getApplicationID().then((appId) => {
       *   // use appId for further actions or assertions
       * });
       */
      getApplicationID(): Chainable<string>;
    }
  }
}

export {};
