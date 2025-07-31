namespace UserService.Tests.FunctionalTests.Helpers;

internal static class GraphQlHelper
{
    public const string RequestAllQuery = """
                                          {
                                            users(skip: 1, take:2 ) {
                                              items {
                                                id
                                                identityId
                                                username
                                                email
                                                lastLoginAt
                                                reputation
                                                createdAt
                                                roles {
                                                  id
                                                  name
                                                }
                                              }
                                              totalCount
                                            }
                                            roles(skip: 1, take:2 ) {
                                              items {
                                                id
                                                name
                                                users {
                                                  id
                                                  username
                                                }
                                              }
                                              totalCount
                                            }
                                          }

                                          """;

    public const string RequestUsersQuery = """
                                            {
                                              users(skip: 1, take:2 ) {
                                                items {
                                                  id
                                                  identityId
                                                  username
                                                  email
                                                  lastLoginAt
                                                  reputation
                                                  createdAt
                                                  roles {
                                                    id
                                                    name
                                                  }
                                                }
                                                totalCount
                                              }
                                            }
                                            """;

    public const string RequestUsersWithInvalidPaginationQuery = """
                                                                 {
                                                                   users(skip:-1, take:101) {
                                                                     items {
                                                                       id
                                                                       identityId
                                                                       username
                                                                       email
                                                                       lastLoginAt
                                                                       reputation
                                                                       createdAt
                                                                       roles {
                                                                         id
                                                                         name
                                                                       }
                                                                     }
                                                                     totalCount
                                                                   }
                                                                 }
                                                                 """;

    public const string RequestWithWrongArgument = """
                                                   {
                                                     users(wrongArg) {
                                                       items {
                                                         id
                                                         identityId
                                                         username
                                                         email
                                                         lastLoginAt
                                                         reputation
                                                         createdAt
                                                         roles {
                                                           id
                                                           name
                                                         }
                                                       }
                                                       totalCount
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
                   identityId
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