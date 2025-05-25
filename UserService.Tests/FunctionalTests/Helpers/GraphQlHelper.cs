namespace UserService.Tests.FunctionalTests.Helpers;

internal static class GraphQlHelper
{
    public const string RequestAllQuery = """
                                          {
                                            users(pagination:  {
                                             pageNumber: 1,
                                             pageSize: 200
                                            }){
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
                                            roles(pagination:  {
                                             pageNumber: 1,
                                             pageSize: 200
                                            }){
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

    public static string RequestRoleByIdQuery(long id)
    {
      return """
             {
               role(id: $ID){
               id
               name
               users{
                 id
                 username
               }
             }
             }
             """.Replace("$ID", id.ToString());
    }
}