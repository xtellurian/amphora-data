# Constant Infrastructure

## Front Door

In order for the Front Door to work, there must be an existing CName record pointing the frontend domains at the Front Door.

For example,

```
Source 	        master.amphoradata.com
Type 	        CNAME
Destination 	amphora.azurefd.net
```

## Notes:

* Remember to add the Azure DNS Zone nameservers to the NameServers in Name.com
* Migrating away from using the O365 nameservers