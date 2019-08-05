# openapi_client.AmphoraSchemaApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**api_schemas_id_json_schema_get**](AmphoraSchemaApi.md#api_schemas_id_json_schema_get) | **GET** /api/schemas/{id}/JsonSchema | 
[**api_schemas_json_schema_put**](AmphoraSchemaApi.md#api_schemas_json_schema_put) | **PUT** /api/schemas/JsonSchema | 
[**api_schemas_put**](AmphoraSchemaApi.md#api_schemas_put) | **PUT** /api/schemas | 


# **api_schemas_id_json_schema_get**
> api_schemas_id_json_schema_get(id)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraSchemaApi()
id = '' # str |  (default to '')

try:
    api_instance.api_schemas_id_json_schema_get(id)
except ApiException as e:
    print("Exception when calling AmphoraSchemaApi->api_schemas_id_json_schema_get: %s\n" % e)
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | **str**|  | [default to &#39;&#39;]

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **api_schemas_json_schema_put**
> api_schemas_json_schema_put(j_schema=j_schema)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraSchemaApi()
j_schema = openapi_client.JSchema() # JSchema |  (optional)

try:
    api_instance.api_schemas_json_schema_put(j_schema=j_schema)
except ApiException as e:
    print("Exception when calling AmphoraSchemaApi->api_schemas_json_schema_put: %s\n" % e)
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **j_schema** | [**JSchema**](JSchema.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: Not defined

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **api_schemas_put**
> api_schemas_put(schema=schema)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraSchemaApi()
schema = openapi_client.Schema() # Schema |  (optional)

try:
    api_instance.api_schemas_put(schema=schema)
except ApiException as e:
    print("Exception when calling AmphoraSchemaApi->api_schemas_put: %s\n" % e)
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **schema** | [**Schema**](Schema.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: Not defined

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

