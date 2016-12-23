Unity Script WalkOnHuman
====

The scripts to walk on a human model in Unity.

## Description

These scrips enable you to make your characters walk on a human model in Unity by attaching some scripts. Please note that it may be unstable, such as not being able to stand properly because of the beta version.

Just attaching a few scripts will allow you to walk on the doll model. Please note that it may be unstable, such as not being able to stand properly because of the beta version.

## Demo

![Demo of UnityChan runing on UnityChan](https://github.com/CdecPGL/ImageRepository/blob/master/WalkOnHumanDemo.gif?raw=true)

## Requirement

This script is cheked on Unity5.5.

## Usage

- Configuration of the human model for ground

    Attach BoneGravityController and HumanColliderController.

    If you want the Collider to reflect the shape change when the shape of the model changes due to animation, set EnableRealtimeUpdate of HumanColliderController to true.

    If EnableRealtimeUpdate is true, performance becomes bad. In this case, you should set EnableTimeLagUpdate to true. There is a possibility this operation may make performance good.

- Configuration of the GameObject you want to walk on the human model

    Please attach GravityReceiver. If gravity is weak, you should chande acceleration in the inspector.

- Configuration of the camera

    Attach AngleFreeFollowCamera and set the game object you want to follow to target.

    You can also adjust the camera position by setting distance and height.

- Configuration of Gravity by Unity's physic engine

    There are some cases that the character can not stand on the human model correctly if the gravity by Unity's physical engine is working.

    In this case, you should set gravitational acceleration to 0 in Edit->Project Settings->Physics. Or you should set UseGravity of RigidBody to false.

    If you can not change them, set ForceToInvalidateRigidBodyGravity of GravityReceiver to true.

## Install

Importe WalkOnHuman.unitypackage in your Unity project.
    
If you also need a sample of these scrips, please import WalkOnHumanWithSample.unitypackege.

## Licence

Based on [UnityChanLicense](http://unity-chan.com/contents/license_en/)

I used [UnityChan](http://unity-chan.com/) in the sample.

![© Unity Technologies Japan/UCL](http://unity-chan.com/images/imageLicenseLogo.png "UCL")

## Author

Cdec
- [GitHub](https://github.com/CdecPGL)
- [HomePage(Japanese)](http://planetagamelabo.com/homepage)

Thank you for being patient with my English.