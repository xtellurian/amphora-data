# openapi_client.AmphoraApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**api_amphorae_get**](AmphoraApi.md#api_amphorae_get) | **GET** /api/amphorae | 
[**api_amphorae_id_drink_get**](AmphoraApi.md#api_amphorae_id_drink_get) | **GET** /api/amphorae/{id}/drink | 
[**api_amphorae_id_fill_post**](AmphoraApi.md#api_amphorae_id_fill_post) | **POST** /api/amphorae/{id}/fill | 
[**api_amphorae_id_get**](AmphoraApi.md#api_amphorae_id_get) | **GET** /api/amphorae/{id} | 
[**api_amphorae_put**](AmphoraApi.md#api_amphorae_put) | **PUT** /api/amphorae | 


# **api_amphorae_get**
> api_amphorae_get()



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraApi()

try:
    api_instance.api_amphorae_get()
except ApiException as e:
    print("Exception when calling AmphoraApi->api_amphorae_get: %s\n" % e)
```

### Parameters
This endpoint does not need any parameter.

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

# **api_amphorae_id_drink_get**
> api_amphorae_id_drink_get(id)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraApi()
id = '' # str |  (default to '')

try:
    api_instance.api_amphorae_id_drink_get(id)
except ApiException as e:
    print("Exception when calling AmphoraApi->api_amphorae_id_drink_get: %s\n" % e)
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

# **api_amphorae_id_fill_post**
> api_amphorae_id_fill_post(id)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraApi()
id = '' # str |  (default to '')

try:
    api_instance.api_amphorae_id_fill_post(id)
except ApiException as e:
    print("Exception when calling AmphoraApi->api_amphorae_id_fill_post: %s\n" % e)
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

# **api_amphorae_id_get**
> api_amphorae_id_get(id)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraApi()
id = '' # str |  (default to '')

try:
    api_instance.api_amphorae_id_get(id)
except ApiException as e:
    print("Exception when calling AmphoraApi->api_amphorae_id_get: %s\n" % e)
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

# **api_amphorae_put**
> api_amphorae_put(amphora=amphora)



### Example

```python
from __future__ import print_function
import time
import openapi_client
from openapi_client.rest import ApiException
from pprint import pprint

# Create an instance of the API class
api_instance = openapi_client.AmphoraApi()
amphora = openapi_client.Amphora() # Amphora |  (optional)

try:
    api_instance.api_amphorae_put(amphora=amphora)
except ApiException as e:
    print("Exception when calling AmphoraApi->api_amphorae_put: %s\n" % e)
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **amphora** | [**Amphora**](Amphora.md)|  | [optional] 

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

