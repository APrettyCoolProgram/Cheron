## ## application-data/dungine/document/

The `application-data/dungine/document/` folder contains

| Folder name         | Folder description                                     |
|--------------------:|:-------------------------------------------------------|
|`development/`       | Dungine development documents                          |
|`manual/`            | Dungine manuals                                        |

Each `application-data/dungine/document/*` folder may contain subfolders for specific documents.

### Copying data locally at runtime
##### Visual Studio 2019
To copy data located in `application-data/asset/*/included/` locally at runtime, set the file properties as such:
```
Build Action: None
Copy to Output Directory: Copy always
```

### Embedding data as a project resource
##### Visual Studio 2019
To build data located in `application-data/asset/*/embedded/` as a project resource, set the file properties as such:
```
Build Action: Resource
```.