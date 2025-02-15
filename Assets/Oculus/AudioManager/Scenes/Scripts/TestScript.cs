/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.Collections;

namespace OVR
{

public class TestScript : MonoBehaviour {

    [InspectorNote( "Sound Setup", "Press '1' to play testSound1 and '2' to play testSound2")]

    public SoundFXRef       testSound1;
    public SoundFXRef       testSound2;

    // Use this for initialization
    void Start () {
    
    }
    

    // Update is called once per frame
    void Update () 
    {
        // use attached game object location
        if ( Input.GetKeyDown( KeyCode.Alpha1 ) ) 
        {
            testSound1.PlaySoundAt( transform.position );
        }

        // hard code information
        if ( Input.GetKeyDown( KeyCode.Alpha2 ) ) {
            testSound2.PlaySoundAt( new Vector3( 5.0f, 0.0f, 0.0f ) );
        }
    }
}

} // namespace OVR
