namespace UserService.Tests.Constants;

public static class GraphQlConstants
{
    public const string RequestAllQuery = """
                                          {
                                            users{
                                              id
                                              keycloakId
                                              username
                                              email
                                              lastLoginAt
                                              reputation
                                              createdAt
                                              roles{
                                                id
                                                name
                                              }
                                            }
                                            roles{
                                              id
                                              name
                                              users{
                                                id
                                                username
                                              }
                                            }
                                          }
                                          """;

    public const string GraphQlEndpoint = "/graphql";
}