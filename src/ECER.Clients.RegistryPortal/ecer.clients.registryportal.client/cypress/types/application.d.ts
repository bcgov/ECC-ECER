declare global {
  namespace Cypress {
    interface Chainable<Subject = any> {
      /**
       * Custom command to seed applications and certificates
       * @example cy.seedRenewalApplication()
       */
      seedRenewalApplication(applicationType?: string, IsActive?: boolean, IsExpiredMoreThan5Years?: boolean): Chainable<Response<any>>;
    }
  }
}

export {};
