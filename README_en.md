Unity Script WalkOnHuman
====

The scripts to walk on a human model in Unity.

## Description

These scrips enable you to make your characters walk on a human model in Unity by attaching some scripts. Please note that it may be unstable, such as not being able to stand properly because this is beta version.

## Demo

![Demo of UnityChan runing on UnityChan](https://github.com/CdecPGL/ImageRepository/blob/master/WalkOnHumanDemo.gif?raw=true)

## Requirement

This script is cheked on Unity5.5.

Because the scripts are using bones to set the gravity for the character to stand on the human model, the human model you want to use as ground must have bones.

## Usage

- Configuration of the human model for ground

    Attach BoneGravityController and HumanColliderController.

    If you want the Collider to reflect the shape change when the shape of the model changes due to animation, set EnableRealtimeUpdate of HumanColliderController to true.

    When EnableRealtimeUpdate is true, performance becomes bad. In this case, you should set EnableTimeLagUpdate to true. There is a possibility this operation may make performance good in exchange for accuracy.

- Configuration of the GameObject you want to walk on the human model

    Please attach GravityReceiver. If gravity is weak, you should change acceleration in the inspector.

- Configuration of the camera

    Attach AngleFreeFollowCamera and set the game object you want the camera to follow to target.

    You can also adjust the camera position by setting distance and height.

- Configuration of Gravity by Unity's physics engine

    There are some cases that the character can not stand on the human model correctly if the gravity by Unity's physics engine is working.

    In this case, you should set gravitational acceleration to 0 in Edit->Project Settings->Physics. Or you should set UseGravity of RigidBody to false.

    If you can not change them, set ForceToInvalidateRigidBodyGravity of GravityReceiver to true.

## Install

Import WalkOnHuman.unitypackage to your Unity project.
    
If you also need a sample of these scrips, please import WalkOnHumanWithSample.unitypackege.

You can download these packages from [the release page](https://github.com/CdecPGL/Unity-Script-WalkOnHuman/releases).

## Licence

Based on [UnityChanLicense](http://unity-chan.com/contents/license_en/)

I used [UnityChan](http://unity-chan.com/) in the sample.

![© Unity Technologies Japan/UCL](http://unity-chan.com/images/imageLicenseLogo.png "UCL")

## Author

Cdec
- [GitHub](https://github.com/CdecPGL)
- [HomePage(Japanese)](http://planetagamelabo.com/homepage)

Thank you for being patient with my English.