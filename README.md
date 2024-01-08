# CrownetUnity
This repository is the Unity project for the [Crownet](https://crownet.org/) project of the University of Applied Sciences. In combination with the [Event Controller and Logger](https://github.com/skiunke/EventControllerAndLogger?tab=readme-ov-file) it allows the 3D rendering and updating of persons, people and 5G network communication from prerecorded files in JSON, InfluxDb or directly from OMNET++.

### Dependencies
- [Unity](https://unity.com/download) (tested with 2022.3.16f1)
- [ECAL](https://github.com/skiunke/EventControllerAndLogger?tab=readme-ov-file)
- [git lfs](https://git-lfs.com/) for demo 

### Installation
```shell
git clone https://github.com/skiunke/CrownetUnity crownetUnity
```
#### Optional:
The demo scene is stored as lfs and contains a ready-to-use scene from the Munich Freiheit.
```shell
git lfs install
git lfs pull
```

### Usage
Start Unity, select a project from disk and select `crownetUnity`. This will automatically resolve all dependencies and build steps and in general load the project. If you run into a problem with assets not being found or the sample scene not loading correctly simply reimport the assets from the dropdown menu by right clicking inside the editor.

### Config
Unity itself does not contain any configurable logic as it is only a display output for the `ECAL` project. However if you intend the model for vehicles, person or the antenna is adjustable and the scaling in general directly inside the editor.
