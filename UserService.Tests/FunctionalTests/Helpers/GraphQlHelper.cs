namespace UserService.Tests.FunctionalTests.Helpers;

public static class GraphQlHelper
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

    public static string RequestUserByIdQuery(long id)
    {
      return """
             {
               user(id: $ID){
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
             }
             """.Replace("$ID", id.ToString());
    }
}