import * as signalR from "@microsoft/signalr";

export function createOrdersHub(baseUrl: string, token?: string) {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${baseUrl}/hubs/orders`, {
      accessTokenFactory: token ? () => token : undefined,
    })
    .withAutomaticReconnect()
    .build();
}
