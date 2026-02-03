namespace UserService.Tests.FunctionalTests.Helpers;

internal static class GraphQlHelper
{
  public const string GraphQlEndpoint = "/graphql";

    public const string RequestAllQuery = """
                                          {
                                            users(skip: 1, take: 5, order: [{ createdAt: DESC }]) {
                                              items {
                                                id
                                                identityId
                                                username
                                                email
                                                lastLoginAt
                                                currentReputation
                                                remainingReputation
                                                initiatedReputationRecords {
                                                  reputationTarget {
                                                    id
                                                    username
                                                  }
                                                  initiator {
                                                    id
                                                    username
                                                  }
                                                  entityId
                                                  reputationRule {
                                                    entityType
                                                    eventType
                                                    reputationTarget
                                                    reputationChange
                                                    reputationRecords {
                                                      id
                                                    }
                                                  }
                                                }
                                                ownedReputationRecords {
                                                  reputationTarget {
                                                    id
                                                    username
                                                  }
                                                  initiator {
                                                    id
                                                    username
                                                  }
                                                  entityId
                                                  reputationRule {
                                                    entityType
                                                    eventType
                                                    reputationTarget
                                                    reputationChange
                                                    reputationRecords {
                                                      id
                                                    }
                                                  }
                                                }
                                                createdAt
                                                roles {
                                                  id
                                                  name
                                                  users {
                                                    id
                                                    username
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }
                                            roles(skip: 1, take: 2, order: [{ name: ASC }]) {
                                              items {
                                                id
                                                name
                                                users {
                                                  id
                                                  username
                                                  roles {
                                                    name
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }
                                            reputationRecords(first: 5, order: [{ createdAt: ASC }]) {
                                              edges {
                                                cursor
                                                node {
                                                  id
                                                  reputationTarget {
                                                    id
                                                    username
                                                  }
                                                  initiator {
                                                    id
                                                    username
                                                  }
                                                  entityId
                                                  reputationRule {
                                                    entityType
                                                    eventType
                                                    reputationTarget
                                                    reputationChange
                                                    reputationRecords {
                                                      id
                                                   }
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                hasNextPage
                                                hasPreviousPage
                                                startCursor
                                                endCursor
                                              }
                                              totalCount
                                            }
                                            reputationRules(take: 10) {
                                              items {
                                                id
                                                entityType
                                                eventType
                                                reputationTarget
                                                reputationChange
                                                reputationRecords {
                                                  id
                                                  reputationTarget {
                                                    id
                                                    username
                                                  }
                                                  initiator {
                                                    id
                                                    username
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }
                                          }
                                          """;

    public const string RequestUsersQuery = """
                                            {
                                              users(skip: 1, take: 2) {
                                                items {
                                                  id
                                                  identityId
                                                  username
                                                  email
                                                  lastLoginAt
                                                  currentReputation
                                                  remainingReputation
                                                  initiatedReputationRecords {
                                                    reputationTarget {
                                                      id
                                                      username
                                                    }
                                                    initiator {
                                                      id
                                                      username
                                                    }
                                                    entityId
                                                    reputationRule {
                                                      entityType
                                                      eventType
                                                      reputationTarget
                                                      reputationChange
                                                      reputationRecords {
                                                        id
                                                      }
                                                    }
                                                  }
                                                  ownedReputationRecords {
                                                    reputationTarget {
                                                      id
                                                      username
                                                    }
                                                    initiator {
                                                      id
                                                      username
                                                    }
                                                    entityId
                                                    reputationRule {
                                                      entityType
                                                      eventType
                                                      reputationTarget
                                                      reputationChange
                                                      reputationRecords {
                                                        id
                                                      }
                                                    }
                                                  }
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
                                                                   users(skip:-1, take:-1) {
                                                                     items {
                                                                       id
                                                                       identityId
                                                                       username
                                                                       email
                                                                       lastLoginAt
                                                                       currentReputation
                                                                       remainingReputation
                                                                       initiatedReputationRecords {
                                                                          reputationTarget {
                                                                            id
                                                                            username
                                                                          }
                                                                          initiator {
                                                                            id
                                                                            username
                                                                          }
                                                                          entityId
                                                                          reputationRule {
                                                                            entityType
                                                                            eventType
                                                                            reputationTarget
                                                                            reputationChange
                                                                            reputationRecords {
                                                                              id
                                                                            }
                                                                          }
                                                                        }
                                                                        ownedReputationRecords {
                                                                          reputationTarget {
                                                                            id
                                                                            username
                                                                          }
                                                                          initiator {
                                                                            id
                                                                            username
                                                                          }
                                                                          entityId
                                                                          reputationRule {
                                                                            entityType
                                                                            eventType
                                                                            reputationTarget
                                                                            reputationChange
                                                                            reputationRecords {
                                                                              id
                                                                            }
                                                                          }
                                                                        }
                                                                       createdAt
                                                                       roles {
                                                                         id
                                                                         name
                                                                         users {
                                                                           id
                                                                           username
                                                                         }
                                                                       }
                                                                     }
                                                                     pageInfo {
                                                                       hasNextPage
                                                                       hasPreviousPage
                                                                     }
                                                                     totalCount
                                                                   }
                                                                   roles(skip: -1, take: 101, order: [{ name: ASC }]) {
                                                                     items {
                                                                       id
                                                                       name
                                                                       users {
                                                                         id
                                                                         username
                                                                         roles {
                                                                           name
                                                                         }
                                                                       }
                                                                     }
                                                                     pageInfo {
                                                                       hasNextPage
                                                                       hasPreviousPage
                                                                     }
                                                                     totalCount
                                                                   }
                                                                   reputationRecords(after: "notValidAfter", first: -1) {
                                                                     edges {
                                                                       cursor
                                                                       node {
                                                                         id
                                                                         reputationTarget {
                                                                            id
                                                                            username
                                                                          }
                                                                          initiator {
                                                                            id
                                                                            username
                                                                          }
                                                                         entityId
                                                                         reputationRule {
                                                                           entityType
                                                                           eventType
                                                                           reputationTarget
                                                                           reputationChange
                                                                           reputationRecords {
                                                                             id
                                                                          }
                                                                         }
                                                                       }
                                                                     }
                                                                     pageInfo {
                                                                       hasNextPage
                                                                       hasPreviousPage
                                                                       startCursor
                                                                       endCursor
                                                                     }
                                                                     totalCount
                                                                   }
                                                                   reputationRules(take: -1) {
                                                                     items {
                                                                       id
                                                                       entityType
                                                                       eventType
                                                                       reputationTarget
                                                                       reputationChange
                                                                       reputationRecords {
                                                                         id
                                                                         reputationTarget {
                                                                            id
                                                                            username
                                                                          }
                                                                          initiator {
                                                                            id
                                                                            username
                                                                          }
                                                                       }
                                                                     }
                                                                     pageInfo {
                                                                       hasNextPage
                                                                       hasPreviousPage
                                                                     }
                                                                     totalCount
                                                                   }
                                                                 }
                                                                 """;

    public const string RequestWithWrongArgument = """
                                                   users(wrongArg) {
                                                     items {
                                                       id
                                                       identityId
                                                       username
                                                       email
                                                       lastLoginAt
                                                       currentReputation
                                                       remainingReputation
                                                       initiatedReputationRecords {
                                                          reputationTarget {
                                                            id
                                                            username
                                                          }
                                                          initiator {
                                                            id
                                                            username
                                                          }
                                                          entityId
                                                          reputationRule {
                                                            entityType
                                                            eventType
                                                            reputationTarget
                                                            reputationChange
                                                            reputationRecords {
                                                              id
                                                            }
                                                          }
                                                        }
                                                        ownedReputationRecords {
                                                          reputationTarget {
                                                            id
                                                            username
                                                          }
                                                          initiator {
                                                            id
                                                            username
                                                          }
                                                          entityId
                                                          reputationRule {
                                                            entityType
                                                            eventType
                                                            reputationTarget
                                                            reputationChange
                                                            reputationRecords {
                                                              id
                                                            }
                                                          }
                                                        }
                                                       createdAt
                                                       roles {
                                                         id
                                                         name
                                                         users {
                                                           id
                                                           username
                                                         }
                                                       }
                                                     }
                                                     pageInfo {
                                                       hasNextPage
                                                       hasPreviousPage
                                                     }
                                                     totalCount
                                                   }
                                                   """;

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
                   currentReputation
                   remainingReputation
                   createdAt
                   roles{
                     id
                     name
                   }
                 }
               }
               """.Replace("$ID", id.ToString());
    }

    public static string RequestAllByIdsQuery(
      long userId,
      long roleId,
      long reputationRecordId,
      long reputationRuleId)
    {
        return """
            {
              user: user(id: $USER_ID) {
                id
                identityId
                username
                email
                lastLoginAt
                currentReputation
                remainingReputation
                initiatedReputationRecords {
                  reputationTarget {
                    id
                    username
                  }
                  initiator {
                    id
                    username
                  }
                  entityId
                  reputationRule {
                    entityType
                    eventType
                    reputationTarget
                    reputationChange
                    reputationRecords {
                      id
                    }
                  }
                }
                ownedReputationRecords {
                  reputationTarget {
                    id
                    username
                  }
                  initiator {
                    id
                    username
                  }
                  entityId
                  reputationRule {
                    entityType
                    eventType
                    reputationTarget
                    reputationChange
                    reputationRecords {
                      id
                    }
                  }
                }
                createdAt
                roles {
                  id
                  name
                }
              }

              role: role(id: $ROLE_ID) {
                id
                name
                users {
                  id
                  username
                }
              }

              reputationRecord: reputationRecord(id: $REPUTATION_RECORD_ID) {
                reputationTarget {
                  id
                  username
                }
                initiator {
                  id
                  username
                }
                entityId
                reputationRule {
                  entityType
                  eventType
                  reputationTarget
                  reputationChange
                  reputationRecords {
                    id
                  }
                }
              }

              reputationRule: reputationRule(id: $REPUTATION_RULE_ID) {
                id
                entityType
                eventType
                reputationTarget
                reputationChange
                reputationRecords {
                  id
                  reputationTarget {
                    id
                    username
                  }
                  initiator {
                    id
                    username
                  }
                }
              }
            }
            """
          .Replace("$USER_ID", userId.ToString())
          .Replace("$ROLE_ID", roleId.ToString())
          .Replace("$REPUTATION_RECORD_ID", reputationRecordId.ToString())
          .Replace("$REPUTATION_RULE_ID", reputationRuleId.ToString());
    }
}