##### Reference this Amphora

```py
from amphora.client import AmphoraDataRepositoryClient, Credentials
credentials = Credentials(username='{{username}}', password='YOUR_PASSWORD')
client = AmphoraDataRepositoryClient(credentials)
amphora = client.get_amphora('{{id}}')
```

##### Download Files

```py
files = amphora.get_files()
print(f'There are {len(files)} files')

if len(files) > 0:
    file_name = files[0].name
    amphora.get_file(file_name).pull(f"/a/local/path/{file_name}")
```

##### Signals to Pandas DataFrame

```py
df = amphora.get_signals().pull().to_pandas()
df.describe()
```