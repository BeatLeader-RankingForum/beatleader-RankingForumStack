Cypress.Commands.add("loginForTesting", () => {
  const userid = "76561198051924392";
  const token = "test";

  cy.request({
    method: "POST",
    url: "https://rfapi.lightai.dev/User/login",
    body: {
      id: userid,
      authToken: token,
    },
  }).then((resp) => {
    Cypress.env("forumAuth", resp.body.jwtToken);
    Cypress.env("forumRefreshToken", resp.body.refreshToken);
  });
});

Cypress.Commands.add("deleteTestReply", () => {
  cy.request({
    method: "DELETE",
    url: "https://rfapi.lightai.dev/Reply/" + Cypress.env("replyId"),
    headers: {
      Authorization: "Bearer " + Cypress.env("forumAuth"),
    },
  }).then((response) => {
    expect(response.status).to.equal(200);
    cy.log("Cleanup action completed successfully.");
  });
});
