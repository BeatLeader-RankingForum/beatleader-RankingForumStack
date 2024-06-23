describe("post-reply", () => {
  before(() => {
    cy.loginForTesting();
  });

  beforeEach(() => {
    cy.visit("https://rankingforum.lightai.dev/rankingforumtest");

    window.localStorage.setItem("forumAuth", Cypress.env("forumAuth"));
    window.localStorage.setItem(
      "forumRefreshToken",
      Cypress.env("forumRefreshToken")
    );
  });

  it("Can send a reply to a comment", () => {
    cy.get(".difficultyOption").click();
    cy.get(".comment-sort-choices").children("div").eq(2).click();
    cy.get(".comment-box").should("exist");
    cy.get(".reply-form").children("textarea").type("Test reply");
    cy.get(".reply-form").children("button").click();
  });

  it("Can see the reply it sent", () => {
    cy.get(".difficultyOption").click();
    cy.get(".comment-sort-choices").children("div").eq(2).click();
    cy.get(".comment-box")
      .children("div")
      .children(".comment-box")
      .eq(1)
      .children("p")
      .should("contain", "Test reply");
    cy.get(".comment-box")
      .children("div")
      .children(".comment-box")
      .eq(1)
      .children("h3")
      .find("small")
      .invoke("text")
      .then((text) => {
        const id = text.match(/reply id: ([\w-]+)/)[1];
        Cypress.env("replyId", id);
      });
  });

  after(() => {
    cy.deleteTestReply();
  });
});
