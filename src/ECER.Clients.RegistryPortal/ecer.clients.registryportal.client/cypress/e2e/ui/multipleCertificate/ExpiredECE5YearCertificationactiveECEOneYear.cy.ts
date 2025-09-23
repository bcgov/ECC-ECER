import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay } from "../../../support/utils";

describe(" Expired  ECE 5 Year Certificate Application Active One Yaar ", () => {
  it("should ECE Five Year beVisbile", () => {
    cy.seedRenewalApplication("ECE5Years", false, false);

    cy.reload();
    /** Dashboard */
    cy.seedRenewalApplication("ECEOneYear", false, false);

    cy.reload();  
    cy.contains("Early Childhood Educator - ECE Five Year").should("be.visible");
  });
   
});
