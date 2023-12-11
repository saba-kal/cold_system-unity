// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/HDRP/Grass Snow"
{
	/*CustomNodeUI:HDPBR*/
    Properties
    {
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_Cutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[Header(Base)]_Glossiness("Base Smoothness", Range( 0 , 1)) = 0
		_Color("Base Color", Color) = (1,1,1,1)
		_BaseBottomColor("Base Bottom Color", Color) = (1,1,1,0)
		_BaseBottomOffset("Base Bottom Offset", Range( 0 , 5)) = 5
		[NoScaleOffset]_MainTex("Albedo (A Opacity)", 2D) = "gray" {}
		[NoScaleOffset][Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		[Header(Top)]_TopIntensity("Top Intensity", Range( 0 , 1)) = 1
		_TopOffset("Top Offset", Range( 0 , 1)) = 0.5
		_TopContrast("Top Contrast", Range( 0 , 2)) = 1
		_TopColor("Top Color", Color) = (1,1,1,0)
		[Header(Tint Color)]_TintColor1("Tint Color 1", Color) = (1,1,1,0)
		_TintColor2("Tint Color 2", Color) = (1,1,1,0)
		_TintNoiseTile("Tint Noise Tile", Range( 0.001 , 30)) = 10
		[Header(Translucency)]_Thickness1("Translucency Thickness", Range( 0 , 1)) = 0.5
		[Header(Wind)]_WindAmplitude("Wind Amplitude", Range( 0 , 3)) = 1
		_WindSpeed("Wind Speed", Range( 0 , 10)) = 5
		_WindScale("Wind Scale", Range( 0 , 30)) = 10
		_WindGrassStiffness("Wind Grass Stiffness", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

		
        Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        
		Cull Off
		Blend One Zero
		ZTest LEqual
		ZWrite On
		ZClip [_ZClip]

		HLSLINCLUDE
		#pragma target 4.5
		#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
		#pragma multi_compile_instancing
		#pragma instancing_options renderinglayer
		#pragma multi_compile _ LOD_FADE_CROSSFADE

		struct GlobalSurfaceDescription
		{
			//Standard
			float3 Albedo;
			float3 Normal;
			float3 Specular;
			float Metallic;
			float3 Emission;
			float Smoothness;
			float Occlusion;
			float Alpha;
			float AlphaClipThreshold;
			float CoatMask;
			//SSS
			float DiffusionProfile;
			float SubsurfaceMask;
			//Transmission
			float Thickness;
			// Anisotropic
			float3 TangentWS;
			float Anisotropy; 
			//Iridescence
			float IridescenceThickness;
			float IridescenceMask;
			// Transparency
			float IndexOfRefraction;
			float3 TransmittanceColor;
			float TransmittanceAbsorptionDistance;
			float TransmittanceMask;
		};

		struct AlphaSurfaceDescription
		{
			float Alpha;
			float AlphaClipThreshold;
		};

		ENDHLSL
		
        Pass
        {
			
            Name "GBuffer"
            Tags { "LightMode"="GBuffer" }    
			Stencil
			{
				Ref 2
				WriteMask 51
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}

     
            HLSLPROGRAM
        	#define _MATERIAL_FEATURE_TRANSMISSION 1
        	#define _NORMALMAP 1
        	#define _ALPHATEST_ON 1
        	#define ASE_SRP_VERSION 70105

        	//#define UNITY_MATERIAL_LIT
			#pragma vertex Vert
			#pragma fragment Frag
			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_GBUFFER
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile DECALS_OFF DECALS_3RT DECALS_4RT
			#pragma multi_compile _ LIGHT_LAYERS
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS


            struct AttributesMesh 
			{
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct PackedVaryingsMeshToPS 
			{
                float4 positionCS : SV_Position;
                float3 interp00 : TEXCOORD0;
                float3 interp01 : TEXCOORD1;
                float4 interp02 : TEXCOORD2;
                float4 interp03 : TEXCOORD3;
				float4 interp04 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };
        
			half AGW_WindGrassScale;
			half AGW_WindGrassSpeed;
			half AGW_WindToggle;
			half AGW_WindGrassAmplitude;
			half AGW_WindGrassStiffness;
			half3 AGW_WindDirection;
			sampler2D _MainTex;
			sampler2D AG_TintNoiseTexture;
			half AG_TintNoiseTile;
			half AG_TintNoiseContrast;
			half AG_TintToggle;
			sampler2D _BumpMap;
			half AGG_SnowOffset;
			half AGG_SnowContrast;
			half AGG_SnowIntensity;
			half AGH_SnowMinimumHeight;
			half AGH_SnowFadeHeight;
			CBUFFER_START( UnityPerMaterial )
			half _WindScale;
			half _WindSpeed;
			half _WindAmplitude;
			half _WindGrassStiffness;
			half4 _BaseBottomColor;
			half4 _Color;
			half4 _TintColor1;
			half4 _TintColor2;
			half _TintNoiseTile;
			float4 _TopColor;
			half _TopOffset;
			half _TopContrast;
			half _TopIntensity;
			half _BaseBottomOffset;
			half _Glossiness;
			half _Cutoff;
			float _Thickness1;
			CBUFFER_END

			
			
			void BuildSurfaceData ( FragInputs fragInputs, GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData )
			{
				ZERO_INITIALIZE ( SurfaceData, surfaceData );

				float3 normalTS = float3( 0.0f, 0.0f, 1.0f );
				normalTS = surfaceDescription.Normal;
				float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				GetNormalWS ( fragInputs, normalTS, surfaceData.normalWS ,doubleSidedConstants);

				surfaceData.ambientOcclusion = 1.0f;

				surfaceData.baseColor = surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness = surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion = surfaceDescription.Occlusion;

				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				surfaceData.specularColor = surfaceDescription.Specular;
				#else
				surfaceData.metallic = surfaceDescription.Metallic;
				#endif

				#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceData.diffusionProfileHash = asuint(surfaceDescription.DiffusionProfile);
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				surfaceData.subsurfaceMask = surfaceDescription.SubsurfaceMask;
				#else
				surfaceData.subsurfaceMask = 1.0f;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				surfaceData.thickness = surfaceDescription.Thickness;
				#endif

				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				surfaceData.anisotropy = surfaceDescription.Anisotropy;

				#else
				surfaceData.anisotropy = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				surfaceData.coatMask = surfaceDescription.CoatMask;
				#else
				surfaceData.coatMask = 0.0f;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				surfaceData.iridescenceThickness = surfaceDescription.IridescenceThickness;
				surfaceData.iridescenceMask = surfaceDescription.IridescenceMask;
				#else
				surfaceData.iridescenceThickness = 0.0;
				surfaceData.iridescenceMask = 1.0;
				#endif

				//ASE CUSTOM TAG
				#ifdef _MATERIAL_FEATURE_TRANSPARENCY
				surfaceData.ior = surfaceDescription.IndexOfRefraction;
				surfaceData.transmittanceColor = surfaceDescription.TransmittanceColor;
				surfaceData.atDistance = surfaceDescription.TransmittanceAbsorptionDistance;
				surfaceData.transmittanceMask = surfaceDescription.TransmittanceMask;
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1000000.0;
				surfaceData.transmittanceMask = 0.0;
				#endif

				surfaceData.specularOcclusion = 1.0;

				#if defined(_BENTNORMALMAP) && defined(_ENABLESPECULAROCCLUSION)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData );
				#elif defined(_MASKMAP)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( NdotV, surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif
				#if HAVE_DECALS
				/*if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData( posInput, surfaceDescription.Alpha );
					ApplyDecalToSurfaceData( decalSurfaceData, surfaceData );
				}*/
				#endif
			}

            void GetSurfaceAndBuiltinData( GlobalSurfaceDescription surfaceDescription , FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
        
				#if _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData );
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal 
				InitBuiltinData (posInput, surfaceDescription.Alpha, surfaceData.normalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);
        
				builtinData.emissiveColor =             surfaceDescription.Emission;
                builtinData.distortion =                float2(0.0, 0.0);           // surfaceDescription.Distortion -- if distortion pass
                builtinData.distortionBlur =            0.0;                        // surfaceDescription.DistortionBlur -- if distortion pass
                builtinData.depthOffset =               0.0;                        // ApplyPerPixelDisplacement(input, V, layerTexCoord, blendMasks); #ifdef _DEPTHOFFSET_ON : ApplyDepthOffsetPositionInput(V, depthOffset, GetWorldToHClipMatrix(), posInput);
        
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);            
            }
        
			PackedVaryingsMeshToPS Vert ( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID ( inputMesh );
				UNITY_TRANSFER_INSTANCE_ID ( inputMesh, outputPackedVaryingsMeshToPS );

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				float mulTime32_g45 = _TimeParameters.x * ( AGW_WindGrassSpeed * _WindSpeed );
				float temp_output_9_0_g45 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
				float temp_output_1_0_g45 = inputMesh.ase_color.r;
				float temp_output_3_0_g45 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g45 ) ) * temp_output_9_0_g45 ) * temp_output_1_0_g45 ) + ( temp_output_9_0_g45 * temp_output_1_0_g45 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
				float3 appendResult70_g45 = (float3(temp_output_3_0_g45 , 0.0 , temp_output_3_0_g45));
				half3 Output_Wind159 = mul( GetWorldToObjectMatrix(), float4( ( appendResult70_g45 * AGW_WindDirection ) , 0.0 ) ).xyz;
				
				float4 temp_cast_2 = (1.0).xxxx;
				float2 appendResult8_g44 = (float2(ase_worldPos.x , ase_worldPos.z));
				float4 lerpResult17_g44 = lerp( _TintColor1 , _TintColor2 , saturate( ( tex2Dlod( AG_TintNoiseTexture, float4( ( appendResult8_g44 * ( 0.001 * AG_TintNoiseTile * _TintNoiseTile ) ), 0, 0.0) ).r * (0.001 + (AG_TintNoiseContrast - 0.001) * (60.0 - 0.001) / (10.0 - 0.001)) ) ));
				float4 lerpResult19_g44 = lerp( temp_cast_2 , lerpResult17_g44 , AG_TintToggle);
				float4 vertexToFrag18_g44 = lerpResult19_g44;
				outputPackedVaryingsMeshToPS.ase_texcoord6 = vertexToFrag18_g44;
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				outputPackedVaryingsMeshToPS.ase_texcoord7.xyz = ase_worldBitangent;
				
				outputPackedVaryingsMeshToPS.ase_texcoord5.xy = inputMesh.ase_texcoord.xy;
				outputPackedVaryingsMeshToPS.ase_color = inputMesh.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord5.zw = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = Output_Wind159;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld ( inputMesh.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldNormal ( inputMesh.normalOS );
				float4 tangentWS = float4( TransformObjectToWorldDir ( inputMesh.tangentOS.xyz ), inputMesh.tangentOS.w );
				float4 positionCS = TransformWorldToHClip ( positionRWS );

				outputPackedVaryingsMeshToPS.positionCS = positionCS;
				outputPackedVaryingsMeshToPS.interp00.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.interp01.xyz = normalWS;
				outputPackedVaryingsMeshToPS.interp02.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.interp03 = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.interp04 = inputMesh.uv2;
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );
				return outputPackedVaryingsMeshToPS;
			}

			void Frag ( PackedVaryingsMeshToPS packedInput, 
						OUTPUT_GBUFFER ( outGBuffer )
						#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
						#endif
						, FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC 
						)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				FragInputs input;
				ZERO_INITIALIZE ( FragInputs, input );
				input.tangentToWorld = k_identity3x3;
				
				float3 positionRWS = packedInput.interp00.xyz;
				float3 normalWS = packedInput.interp01.xyz;
				float4 tangentWS = packedInput.interp02.xyzw;
			
				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
				input.tangentToWorld = BuildTangentToWorld ( tangentWS, normalWS );
				input.texCoord1 = packedInput.interp03;
				input.texCoord2 = packedInput.interp04;

				// input.positionSS is SV_Position
				PositionInputs posInput = GetPositionInput ( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir ( input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;

				GlobalSurfaceDescription surfaceDescription = ( GlobalSurfaceDescription ) 0;
				float2 uv_MainTex1 = packedInput.ase_texcoord5.xy;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
				float4 vertexToFrag18_g44 = packedInput.ase_texcoord6;
				half4 TintColor229 = vertexToFrag18_g44;
				float2 uv_BumpMap23 = packedInput.ase_texcoord5.xy;
				float3 break105_g36 = UnpackNormalmapRGorAG( tex2D( _BumpMap, uv_BumpMap23 ), 1.0f );
				float switchResult107_g36 = (((ase_vface>0)?(break105_g36.z):(-break105_g36.z)));
				float3 appendResult108_g36 = (float3(break105_g36.x , break105_g36.y , switchResult107_g36));
				half3 Output_Normal96 = appendResult108_g36;
				float3 desaturateInitialColor301 = Output_Normal96;
				float desaturateDot301 = dot( desaturateInitialColor301, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar301 = lerp( desaturateInitialColor301, desaturateDot301.xxx, 1.0 );
				float3 ase_worldBitangent = packedInput.ase_texcoord7.xyz;
				float3 tanToWorld0 = float3( tangentWS.xyz.x, ase_worldBitangent.x, normalWS.x );
				float3 tanToWorld1 = float3( tangentWS.xyz.y, ase_worldBitangent.y, normalWS.y );
				float3 tanToWorld2 = float3( tangentWS.xyz.z, ase_worldBitangent.z, normalWS.z );
				float3 tanNormal218 = desaturateVar301;
				float3 worldNormal218 = float3(dot(tanToWorld0,tanNormal218), dot(tanToWorld1,tanNormal218), dot(tanToWorld2,tanNormal218));
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				half TopMask211 = ( saturate( ( pow( abs( ( saturate( worldNormal218.y ) + ( _TopOffset * AGG_SnowOffset ) ) ) , ( (1.0 + (_TopContrast - 0.0) * (200.0 - 1.0) / (1.0 - 0.0)) * AGG_SnowContrast ) ) * ( _TopIntensity * AGG_SnowIntensity ) ) ) * saturate( (0.0 + (ase_worldPos.y - AGH_SnowMinimumHeight) * (1.0 - 0.0) / (( AGH_SnowMinimumHeight + AGH_SnowFadeHeight ) - AGH_SnowMinimumHeight)) ) );
				float4 lerpResult215 = lerp( ( _Color * tex2DNode1 * TintColor229 ) , _TopColor , TopMask211);
				float4 lerpResult8 = lerp( ( _BaseBottomColor * lerpResult215 ) , lerpResult215 , saturate( pow( abs( ( packedInput.ase_color.r * 2.0 ) ) , _BaseBottomOffset ) ));
				half4 Output_Albedo97 = lerpResult8;
				
				half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
				
				surfaceDescription.Albedo = Output_Albedo97.rgb;
				surfaceDescription.Normal = Output_Normal96;
				surfaceDescription.Emission = 0;
				surfaceDescription.Specular = 0;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Smoothness = _Glossiness;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = Output_OpacityMask58;
				surfaceDescription.AlphaClipThreshold = _Cutoff;

				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceDescription.CoatMask = 0;
				#endif

				#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.DiffusionProfile = 2.0088424682617188;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = 1;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceDescription.Thickness = _Thickness1;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceThickness = 0;
				surfaceDescription.IridescenceMask = 1;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSPARENCY
				surfaceDescription.IndexOfRefraction = 1;
				surfaceDescription.TransmittanceColor = float3( 1, 1, 1 );
				surfaceDescription.TransmittanceAbsorptionDistance = 1000000;
				surfaceDescription.TransmittanceMask = 0;
				#endif
				GetSurfaceAndBuiltinData ( surfaceDescription, input, normalizedWorldViewDir, posInput, surfaceData, builtinData );
				ENCODE_INTO_GBUFFER ( surfaceData, builtinData, posInput.positionSS, outGBuffer );
				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif
			}

            ENDHLSL
        }
        
		
		
        Pass
        {
			
            Name "META"
            Tags { "LightMode"="Meta" }
            Cull Off
            HLSLPROGRAM
			#define _MATERIAL_FEATURE_TRANSMISSION 1
			#define _NORMALMAP 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70105

			//#define UNITY_MATERIAL_LIT
			#pragma vertex Vert
			#pragma fragment Frag
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
			#define SHADERPASS SHADERPASS_LIGHT_TRANSPORT
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
			#define ATTRIBUTES_NEED_COLOR
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

            struct AttributesMesh 
			{
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 color : COLOR;
				
            };

            struct PackedVaryingsMeshToPS
			{
                float4 positionCS : SV_Position;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_color : COLOR;
            };
            
			half AGW_WindGrassScale;
			half AGW_WindGrassSpeed;
			half AGW_WindToggle;
			half AGW_WindGrassAmplitude;
			half AGW_WindGrassStiffness;
			half3 AGW_WindDirection;
			sampler2D _MainTex;
			sampler2D AG_TintNoiseTexture;
			half AG_TintNoiseTile;
			half AG_TintNoiseContrast;
			half AG_TintToggle;
			sampler2D _BumpMap;
			half AGG_SnowOffset;
			half AGG_SnowContrast;
			half AGG_SnowIntensity;
			half AGH_SnowMinimumHeight;
			half AGH_SnowFadeHeight;
			CBUFFER_START( UnityPerMaterial )
			half _WindScale;
			half _WindSpeed;
			half _WindAmplitude;
			half _WindGrassStiffness;
			half4 _BaseBottomColor;
			half4 _Color;
			half4 _TintColor1;
			half4 _TintColor2;
			half _TintNoiseTile;
			float4 _TopColor;
			half _TopOffset;
			half _TopContrast;
			half _TopIntensity;
			half _BaseBottomOffset;
			half _Glossiness;
			half _Cutoff;
			float _Thickness1;
			CBUFFER_END

			
			
			void BuildSurfaceData ( FragInputs fragInputs, GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData )
			{
				ZERO_INITIALIZE ( SurfaceData, surfaceData );

				float3 normalTS = float3( 0.0f, 0.0f, 1.0f );
				normalTS = surfaceDescription.Normal;
				float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				GetNormalWS ( fragInputs, normalTS, surfaceData.normalWS ,doubleSidedConstants);

				surfaceData.ambientOcclusion = 1.0f;

				surfaceData.baseColor = surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness = surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion = surfaceDescription.Occlusion;

				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				surfaceData.specularColor = surfaceDescription.Specular;
				#else
				surfaceData.metallic = surfaceDescription.Metallic;
				#endif

				#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceData.diffusionProfileHash = asuint(surfaceDescription.DiffusionProfile);
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				surfaceData.subsurfaceMask = surfaceDescription.SubsurfaceMask;

				#else
				surfaceData.subsurfaceMask = 1.0f;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				surfaceData.thickness = surfaceDescription.Thickness;
				#endif

				surfaceData.tangentWS = normalize ( fragInputs.tangentToWorld[ 0 ].xyz );
				surfaceData.tangentWS = Orthonormalize ( surfaceData.tangentWS, surfaceData.normalWS );

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				surfaceData.anisotropy = surfaceDescription.Anisotropy;

				#else
				surfaceData.anisotropy = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				surfaceData.coatMask = surfaceDescription.CoatMask;
				#else
				surfaceData.coatMask = 0.0f;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				surfaceData.iridescenceThickness = surfaceDescription.IridescenceThickness;
				surfaceData.iridescenceMask = surfaceDescription.IridescenceMask;
				#else
				surfaceData.iridescenceThickness = 0.0;
				surfaceData.iridescenceMask = 1.0;
				#endif

				//ASE CUSTOM TAG
				#ifdef _MATERIAL_FEATURE_TRANSPARENCY
				surfaceData.ior = surfaceDescription.IndexOfRefraction;
				surfaceData.transmittanceColor = surfaceDescription.TransmittanceColor;
				surfaceData.atDistance = surfaceDescription.TransmittanceAbsorptionDistance;
				surfaceData.transmittanceMask = surfaceDescription.TransmittanceMask;
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1000000.0;
				surfaceData.transmittanceMask = 0.0;
				#endif

				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion (ClampNdotV (dot (surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness (surfaceData.perceptualSmoothness));

				#if HAVE_DECALS
				if (_EnableDecals)
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData (posInput, surfaceDescription.Alpha);
					ApplyDecalToSurfaceData (decalSurfaceData, surfaceData);
				}
				#endif

				#if defined(_BENTNORMALMAP) && defined(_ENABLESPECULAROCCLUSION)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO ( V, bentNormalWS, surfaceData );
				#elif defined(_MASKMAP)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion ( NdotV, surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness ( surfaceData.perceptualSmoothness ) );
				#endif
			}

            void GetSurfaceAndBuiltinData( GlobalSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
				#if _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
				BuildSurfaceData (fragInputs, surfaceDescription, V, posInput, surfaceData);
        
				// Builtin Data
                // For back lighting we use the oposite vertex normal 
				InitBuiltinData (posInput, surfaceDescription.Alpha, surfaceData.normalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);
        
		        builtinData.emissiveColor =             surfaceDescription.Emission;
                builtinData.distortion =                float2(0.0, 0.0);           // surfaceDescription.Distortion -- if distortion pass
                builtinData.distortionBlur =            0.0;                        // surfaceDescription.DistortionBlur -- if distortion pass
                builtinData.depthOffset =               0.0;                        // ApplyPerPixelDisplacement(input, V, layerTexCoord, blendMasks); #ifdef _DEPTHOFFSET_ON : ApplyDepthOffsetPositionInput(V, depthOffset, GetWorldToHClipMatrix(), posInput);
        
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
            }
        
           
			CBUFFER_START ( UnityMetaPass )
				bool4 unity_MetaVertexControl;
				bool4 unity_MetaFragmentControl;
			CBUFFER_END


			float unity_OneOverOutputBoost;
			float unity_MaxOutputValue;

			PackedVaryingsMeshToPS Vert ( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID ( inputMesh );
				UNITY_TRANSFER_INSTANCE_ID ( inputMesh, outputPackedVaryingsMeshToPS );

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				float mulTime32_g45 = _TimeParameters.x * ( AGW_WindGrassSpeed * _WindSpeed );
				float temp_output_9_0_g45 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
				float temp_output_1_0_g45 = inputMesh.color.r;
				float temp_output_3_0_g45 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g45 ) ) * temp_output_9_0_g45 ) * temp_output_1_0_g45 ) + ( temp_output_9_0_g45 * temp_output_1_0_g45 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
				float3 appendResult70_g45 = (float3(temp_output_3_0_g45 , 0.0 , temp_output_3_0_g45));
				half3 Output_Wind159 = mul( GetWorldToObjectMatrix(), float4( ( appendResult70_g45 * AGW_WindDirection ) , 0.0 ) ).xyz;
				
				float4 temp_cast_2 = (1.0).xxxx;
				float2 appendResult8_g44 = (float2(ase_worldPos.x , ase_worldPos.z));
				float4 lerpResult17_g44 = lerp( _TintColor1 , _TintColor2 , saturate( ( tex2Dlod( AG_TintNoiseTexture, float4( ( appendResult8_g44 * ( 0.001 * AG_TintNoiseTile * _TintNoiseTile ) ), 0, 0.0) ).r * (0.001 + (AG_TintNoiseContrast - 0.001) * (60.0 - 0.001) / (10.0 - 0.001)) ) ));
				float4 lerpResult19_g44 = lerp( temp_cast_2 , lerpResult17_g44 , AG_TintToggle);
				float4 vertexToFrag18_g44 = lerpResult19_g44;
				outputPackedVaryingsMeshToPS.ase_texcoord1 = vertexToFrag18_g44;
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				outputPackedVaryingsMeshToPS.ase_texcoord2.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				outputPackedVaryingsMeshToPS.ase_texcoord3.xyz = ase_worldNormal;
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				outputPackedVaryingsMeshToPS.ase_texcoord4.xyz = ase_worldBitangent;
				outputPackedVaryingsMeshToPS.ase_texcoord5.xyz = ase_worldPos;
				
				outputPackedVaryingsMeshToPS.ase_texcoord.xy = inputMesh.uv0;
				outputPackedVaryingsMeshToPS.ase_color = inputMesh.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord.zw = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord2.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord3.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord4.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord5.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = Output_Wind159;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float2 uv;

				if ( unity_MetaVertexControl.x )
				{
					uv = inputMesh.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
				}
				else if ( unity_MetaVertexControl.y )
				{
					uv = inputMesh.uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				}

				outputPackedVaryingsMeshToPS.positionCS = float4( uv * 2.0 - 1.0, inputMesh.positionOS.z > 0 ? 1.0e-4 : 0.0, 1.0 );

				return outputPackedVaryingsMeshToPS;
			}

			float4 Frag ( PackedVaryingsMeshToPS packedInput , FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE ( FragInputs, input );
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				PositionInputs posInput = GetPositionInput ( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				float3 V = 0;

				SurfaceData surfaceData;
				BuiltinData builtinData;

				GlobalSurfaceDescription surfaceDescription = ( GlobalSurfaceDescription ) 0;
				float2 uv_MainTex1 = packedInput.ase_texcoord.xy;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
				float4 vertexToFrag18_g44 = packedInput.ase_texcoord1;
				half4 TintColor229 = vertexToFrag18_g44;
				float2 uv_BumpMap23 = packedInput.ase_texcoord.xy;
				float3 break105_g36 = UnpackNormalmapRGorAG( tex2D( _BumpMap, uv_BumpMap23 ), 1.0f );
				float switchResult107_g36 = (((ase_vface>0)?(break105_g36.z):(-break105_g36.z)));
				float3 appendResult108_g36 = (float3(break105_g36.x , break105_g36.y , switchResult107_g36));
				half3 Output_Normal96 = appendResult108_g36;
				float3 desaturateInitialColor301 = Output_Normal96;
				float desaturateDot301 = dot( desaturateInitialColor301, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar301 = lerp( desaturateInitialColor301, desaturateDot301.xxx, 1.0 );
				float3 ase_worldTangent = packedInput.ase_texcoord2.xyz;
				float3 ase_worldNormal = packedInput.ase_texcoord3.xyz;
				float3 ase_worldBitangent = packedInput.ase_texcoord4.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal218 = desaturateVar301;
				float3 worldNormal218 = float3(dot(tanToWorld0,tanNormal218), dot(tanToWorld1,tanNormal218), dot(tanToWorld2,tanNormal218));
				float3 ase_worldPos = packedInput.ase_texcoord5.xyz;
				half TopMask211 = ( saturate( ( pow( abs( ( saturate( worldNormal218.y ) + ( _TopOffset * AGG_SnowOffset ) ) ) , ( (1.0 + (_TopContrast - 0.0) * (200.0 - 1.0) / (1.0 - 0.0)) * AGG_SnowContrast ) ) * ( _TopIntensity * AGG_SnowIntensity ) ) ) * saturate( (0.0 + (ase_worldPos.y - AGH_SnowMinimumHeight) * (1.0 - 0.0) / (( AGH_SnowMinimumHeight + AGH_SnowFadeHeight ) - AGH_SnowMinimumHeight)) ) );
				float4 lerpResult215 = lerp( ( _Color * tex2DNode1 * TintColor229 ) , _TopColor , TopMask211);
				float4 lerpResult8 = lerp( ( _BaseBottomColor * lerpResult215 ) , lerpResult215 , saturate( pow( abs( ( packedInput.ase_color.r * 2.0 ) ) , _BaseBottomOffset ) ));
				half4 Output_Albedo97 = lerpResult8;
				
				half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
				
				surfaceDescription.Albedo = Output_Albedo97.rgb;
				surfaceDescription.Normal = Output_Normal96;
				surfaceDescription.Emission = 0;
				surfaceDescription.Specular = 0;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Smoothness = _Glossiness;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = Output_OpacityMask58;
				surfaceDescription.AlphaClipThreshold = _Cutoff;

				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceDescription.CoatMask = 0;
				#endif

				#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.DiffusionProfile = 2.0088424682617188;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = 1;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceDescription.Thickness = _Thickness1;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceThickness = 0;
				surfaceDescription.IridescenceMask = 1;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSPARENCY
				surfaceDescription.IndexOfRefraction = 1;
				surfaceDescription.TransmittanceColor = float3( 1, 1, 1 );
				surfaceDescription.TransmittanceAbsorptionDistance = 1000000;
				surfaceDescription.TransmittanceMask = 0;
				#endif

				GetSurfaceAndBuiltinData ( surfaceDescription, input, V, posInput, surfaceData, builtinData );

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData ( input.positionSS.xy, surfaceData );

				LightTransportData lightTransportData = GetLightTransportData ( surfaceData, builtinData, bsdfData );

				float4 res = float4( 0.0, 0.0, 0.0, 1.0 );
				if ( unity_MetaFragmentControl.x )
				{
					res.rgb = clamp ( pow ( abs ( lightTransportData.diffuseColor ), saturate ( unity_OneOverOutputBoost ) ), 0, unity_MaxOutputValue );
				}

				if ( unity_MetaFragmentControl.y )
				{
					res.rgb = lightTransportData.emissiveColor;
				}

				return res;
			}
       
            ENDHLSL
        }

		
		Pass
        {
			
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ColorMask 0
			

            HLSLPROGRAM
			#define _MATERIAL_FEATURE_TRANSMISSION 1
			#define _NORMALMAP 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70105

			//#define UNITY_MATERIAL_LIT
			#pragma vertex Vert
			#pragma fragment Frag
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_SHADOWS
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			
        

            struct AttributesMesh 
			{
                float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct PackedVaryingsMeshToPS 
			{
                float4 positionCS : SV_Position;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };
        
			half AGW_WindGrassScale;
			half AGW_WindGrassSpeed;
			half AGW_WindToggle;
			half AGW_WindGrassAmplitude;
			half AGW_WindGrassStiffness;
			half3 AGW_WindDirection;
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			half _WindScale;
			half _WindSpeed;
			half _WindAmplitude;
			half _WindGrassStiffness;
			half4 _BaseBottomColor;
			half4 _Color;
			half4 _TintColor1;
			half4 _TintColor2;
			half _TintNoiseTile;
			float4 _TopColor;
			half _TopOffset;
			half _TopContrast;
			half _TopIntensity;
			half _BaseBottomOffset;
			half _Glossiness;
			half _Cutoff;
			float _Thickness1;
			CBUFFER_END

			
			
            void BuildSurfaceData(FragInputs fragInputs, AlphaSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
            {
                ZERO_INITIALIZE(SurfaceData, surfaceData);
                surfaceData.ambientOcclusion =      1.0f;
                surfaceData.subsurfaceMask =        1.0f;
        
                surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
                float3 normalTS = float3(0.0f, 0.0f, 1.0f);
                float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				GetNormalWS(fragInputs, normalTS, surfaceData.normalWS,doubleSidedConstants);
                surfaceData.tangentWS = normalize(fragInputs.tangentToWorld[0].xyz);
                surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
                surfaceData.anisotropy = 0;
                surfaceData.coatMask = 0.0f;
                surfaceData.iridescenceThickness = 0.0;
                surfaceData.iridescenceMask = 1.0;
                surfaceData.ior = 1.0;
                surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
                surfaceData.atDistance = 1000000.0;
                surfaceData.transmittanceMask = 0.0;
                surfaceData.specularOcclusion = 1.0;
				#if defined(_BENTNORMALMAP) && defined(_ENABLESPECULAROCCLUSION)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData);
				#elif defined(_MASKMAP)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(NdotV, surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#endif
				#if HAVE_DECALS
				if (_EnableDecals)
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData (posInput, surfaceDescription.Alpha);
					ApplyDecalToSurfaceData (decalSurfaceData, surfaceData);
				}
				#endif
            }
        
            void GetSurfaceAndBuiltinData( AlphaSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
				#if _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
                // Builtin Data
                // For back lighting we use the oposite vertex normal 
				InitBuiltinData (posInput, surfaceDescription.Alpha, surfaceData.normalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);
                builtinData.distortion =                float2(0.0, 0.0);           // surfaceDescription.Distortion -- if distortion pass
                builtinData.distortionBlur =            0.0;                        // surfaceDescription.DistortionBlur -- if distortion pass
                builtinData.depthOffset =               0.0;                        // ApplyPerPixelDisplacement(input, V, layerTexCoord, blendMasks); #ifdef _DEPTHOFFSET_ON : ApplyDepthOffsetPositionInput(V, depthOffset, GetWorldToHClipMatrix(), posInput);
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);            
            }

			PackedVaryingsMeshToPS Vert( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID ( inputMesh );
				UNITY_TRANSFER_INSTANCE_ID ( inputMesh, outputPackedVaryingsMeshToPS );

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				float mulTime32_g45 = _TimeParameters.x * ( AGW_WindGrassSpeed * _WindSpeed );
				float temp_output_9_0_g45 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
				float temp_output_1_0_g45 = inputMesh.ase_color.r;
				float temp_output_3_0_g45 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g45 ) ) * temp_output_9_0_g45 ) * temp_output_1_0_g45 ) + ( temp_output_9_0_g45 * temp_output_1_0_g45 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
				float3 appendResult70_g45 = (float3(temp_output_3_0_g45 , 0.0 , temp_output_3_0_g45));
				half3 Output_Wind159 = mul( GetWorldToObjectMatrix(), float4( ( appendResult70_g45 * AGW_WindDirection ) , 0.0 ) ).xyz;
				
				outputPackedVaryingsMeshToPS.ase_texcoord.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = Output_Wind159;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld ( inputMesh.positionOS.xyz );
				float4 positionCS = TransformWorldToHClip ( positionRWS );

				outputPackedVaryingsMeshToPS.positionCS = positionCS;
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput
						#ifdef WRITE_NORMAL_BUFFER
						, out float4 outNormalBuffer : SV_Target0
							#ifdef WRITE_MSAA_DEPTH
							, out float1 depthColor : SV_Target1
							#endif
						#elif defined(WRITE_MSAA_DEPTH) // When only WRITE_MSAA_DEPTH is define and not WRITE_NORMAL_BUFFER it mean we are Unlit and only need depth, but we still have normal buffer binded
						, out float4 outNormalBuffer : SV_Target0
						, out float1 depthColor : SV_Target1
						#else
						, out float4 outColor : SV_Target0
						#endif

						#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
						#endif
						 
						)
				{
					UNITY_SETUP_INSTANCE_ID( packedInput );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
					FragInputs input;
					ZERO_INITIALIZE(FragInputs, input);
					input.tangentToWorld = k_identity3x3;
					input.positionSS = packedInput.positionCS;       // input.positionCS is SV_Position

					// input.positionSS is SV_Position
					PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

					float3 V = float3(1.0, 1.0, 1.0); // Avoid the division by 0

					SurfaceData surfaceData;
					BuiltinData builtinData;
					AlphaSurfaceDescription surfaceDescription = (AlphaSurfaceDescription)0;
					float2 uv_MainTex1 = packedInput.ase_texcoord.xy;
					float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
					half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
					
					surfaceDescription.Alpha = Output_OpacityMask58;
					surfaceDescription.AlphaClipThreshold = _Cutoff;

					GetSurfaceAndBuiltinData(surfaceDescription,input, V, posInput, surfaceData, builtinData);

					#ifdef _DEPTHOFFSET_ON
					outputDepth = posInput.deviceDepth;
					#endif

					#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);
					#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.positionCS.z;
					#endif
					#elif defined(WRITE_MSAA_DEPTH) 
					outNormalBuffer = float4(0.0, 0.0, 0.0, 1.0);
					depthColor = packedInput.vmesh.positionCS.z;
					#elif defined(SCENESELECTIONPASS)
					outColor = float4(_ObjectId, _PassValue, 1.0, 1.0);
					#else
					outColor = float4(0.0, 0.0, 0.0, 0.0);
					#endif
				}
            ENDHLSL
        }
		
		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

            ColorMask 0
        
            HLSLPROGRAM
			#define _MATERIAL_FEATURE_TRANSMISSION 1
			#define _NORMALMAP 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70105

			//#define UNITY_MATERIAL_LIT
			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
            #define SCENESELECTIONPASS
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
			
        
			int _ObjectId;
			int _PassValue;
        
			struct AttributesMesh 
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position; 
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
        
			half AGW_WindGrassScale;
			half AGW_WindGrassSpeed;
			half AGW_WindToggle;
			half AGW_WindGrassAmplitude;
			half AGW_WindGrassStiffness;
			half3 AGW_WindDirection;
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			half _WindScale;
			half _WindSpeed;
			half _WindAmplitude;
			half _WindGrassStiffness;
			half4 _BaseBottomColor;
			half4 _Color;
			half4 _TintColor1;
			half4 _TintColor2;
			half _TintNoiseTile;
			float4 _TopColor;
			half _TopOffset;
			half _TopContrast;
			half _TopIntensity;
			half _BaseBottomOffset;
			half _Glossiness;
			half _Cutoff;
			float _Thickness1;
			CBUFFER_END

		
			                
        
			void BuildSurfaceData(FragInputs fragInputs, AlphaSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.ambientOcclusion =      1.0f;
				surfaceData.subsurfaceMask =        1.0f;
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
				float3 normalTS =                   float3(0.0f, 0.0f, 1.0f);
				float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				GetNormalWS(fragInputs, normalTS, surfaceData.normalWS,doubleSidedConstants);
				surfaceData.tangentWS = normalize(fragInputs.tangentToWorld[0].xyz); 
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				surfaceData.anisotropy = 0;
				surfaceData.coatMask = 0.0f;
				surfaceData.iridescenceThickness = 0.0;
				surfaceData.iridescenceMask = 1.0;
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
				surfaceData.atDistance = 1000000.0;
				surfaceData.transmittanceMask = 0.0;
				surfaceData.specularOcclusion = 1.0;
				#if defined(_BENTNORMALMAP) && defined(_ENABLESPECULAROCCLUSION)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData);
				#elif defined(_MASKMAP)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(NdotV, surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#endif
				
				#if HAVE_DECALS
				if (_EnableDecals)
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData (posInput, surfaceDescription.Alpha);
					ApplyDecalToSurfaceData (decalSurfaceData, surfaceData);
				}
				#endif
			}
        
			void GetSurfaceAndBuiltinData(AlphaSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#if _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
				InitBuiltinData (posInput, surfaceDescription.Alpha, surfaceData.normalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);
				builtinData.distortion =                float2(0.0, 0.0);           
				builtinData.distortionBlur =            0.0;                        
				builtinData.depthOffset =               0.0;                        
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}
        
       
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
					
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
					
				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				float mulTime32_g45 = _TimeParameters.x * ( AGW_WindGrassSpeed * _WindSpeed );
				float temp_output_9_0_g45 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
				float temp_output_1_0_g45 = inputMesh.ase_color.r;
				float temp_output_3_0_g45 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g45 ) ) * temp_output_9_0_g45 ) * temp_output_1_0_g45 ) + ( temp_output_9_0_g45 * temp_output_1_0_g45 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
				float3 appendResult70_g45 = (float3(temp_output_3_0_g45 , 0.0 , temp_output_3_0_g45));
				half3 Output_Wind159 = mul( GetWorldToObjectMatrix(), float4( ( appendResult70_g45 * AGW_WindDirection ) , 0.0 ) ).xyz;
				
				outputPackedVaryingsMeshToPS.ase_texcoord.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = Output_Wind159;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
					
				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
			
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput
						#ifdef WRITE_NORMAL_BUFFER
						, out float4 outNormalBuffer : SV_Target0
							#ifdef WRITE_MSAA_DEPTH
							, out float1 depthColor : SV_Target1
							#endif
						#elif defined(WRITE_MSAA_DEPTH) 
						, out float4 outNormalBuffer : SV_Target0
						, out float1 depthColor : SV_Target1
						#elif defined(SCENESELECTIONPASS)
						, out float4 outColor : SV_Target0
						#endif

						#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
						#endif
						
					)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
					

				// input.positionSS is SV_Position
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				
				float3 V = float3(1.0, 1.0, 1.0); // Avoid the division by 0
				
				SurfaceData surfaceData;
				BuiltinData builtinData;
				AlphaSurfaceDescription surfaceDescription = ( AlphaSurfaceDescription ) 0;
				float2 uv_MainTex1 = packedInput.ase_texcoord.xy;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
				half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
				
				surfaceDescription.Alpha = Output_OpacityMask58;
				surfaceDescription.AlphaClipThreshold = _Cutoff;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif

				#ifdef WRITE_NORMAL_BUFFER
				EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);
				#ifdef WRITE_MSAA_DEPTH
				depthColor = packedInput.positionCS.z;
				#endif
				#elif defined(WRITE_MSAA_DEPTH) 
				outNormalBuffer = float4(0.0, 0.0, 0.0, 1.0);
				depthColor = packedInput.vmesh.positionCS.z;
				#elif defined(SCENESELECTIONPASS)
				outColor = float4(_ObjectId, _PassValue, 1.0, 1.0);
				#endif
			}

            ENDHLSL
        }
		
        Pass
        {
			
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
			Stencil
			{
				Ref 0
				WriteMask 48
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}

            
            HLSLPROGRAM
			#define _MATERIAL_FEATURE_TRANSMISSION 1
			#define _NORMALMAP 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70105

			//#define UNITY_MATERIAL_LIT
			#pragma vertex Vert
			#pragma fragment Frag
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
			#define SHADERPASS SHADERPASS_DEPTH_ONLY
			#pragma multi_compile _ WRITE_NORMAL_BUFFER
			#pragma multi_compile _ WRITE_MSAA_DEPTH

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define ATTRIBUTES_NEED_TEXCOORD1
			#define ATTRIBUTES_NEED_TEXCOORD2
			#define ATTRIBUTES_NEED_TEXCOORD3
			#define ATTRIBUTES_NEED_COLOR
			#define VARYINGS_NEED_POSITION_WS
			#define VARYINGS_NEED_TANGENT_TO_WORLD
			#define VARYINGS_NEED_TEXCOORD0
			#define VARYINGS_NEED_TEXCOORD1
			#define VARYINGS_NEED_TEXCOORD2
			#define VARYINGS_NEED_TEXCOORD3
			#define VARYINGS_NEED_COLOR
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
			
				
			struct AttributesMesh 
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			half AGW_WindGrassScale;
			half AGW_WindGrassSpeed;
			half AGW_WindToggle;
			half AGW_WindGrassAmplitude;
			half AGW_WindGrassStiffness;
			half3 AGW_WindDirection;
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			half _WindScale;
			half _WindSpeed;
			half _WindAmplitude;
			half _WindGrassStiffness;
			half4 _BaseBottomColor;
			half4 _Color;
			half4 _TintColor1;
			half4 _TintColor2;
			half _TintNoiseTile;
			float4 _TopColor;
			half _TopOffset;
			half _TopContrast;
			half _TopIntensity;
			half _BaseBottomOffset;
			half _Glossiness;
			half _Cutoff;
			float _Thickness1;
			CBUFFER_END

				
			        
			void BuildSurfaceData(FragInputs fragInputs, AlphaSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.ambientOcclusion =      1.0f;
				surfaceData.subsurfaceMask =        1.0f;

				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
				float3 normalTS =                   float3(0.0f, 0.0f, 1.0f);
				float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				GetNormalWS(fragInputs, normalTS, surfaceData.normalWS,doubleSidedConstants);
				surfaceData.tangentWS = normalize(fragInputs.tangentToWorld[0].xyz);    // The tangent is not normalize in tangentToWorld for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				surfaceData.anisotropy = 0;
				surfaceData.coatMask = 0.0f;
				surfaceData.iridescenceThickness = 0.0;
				surfaceData.iridescenceMask = 1.0;
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
				surfaceData.atDistance = 1000000.0;
				surfaceData.transmittanceMask = 0.0;
				surfaceData.specularOcclusion = 1.0;
				#if defined(_BENTNORMALMAP) && defined(_ENABLESPECULAROCCLUSION)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData);
				#elif defined(_MASKMAP)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(NdotV, surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#endif
				#if HAVE_DECALS
				if (_EnableDecals)
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData (posInput, surfaceDescription.Alpha);
					ApplyDecalToSurfaceData (decalSurfaceData, surfaceData);
				}
				#endif
			}
        
			void GetSurfaceAndBuiltinData(AlphaSurfaceDescription surfaceDescription,FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#if _ALPHATEST_ON
					DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
				BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
				InitBuiltinData (posInput, surfaceDescription.Alpha, surfaceData.normalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);

				builtinData.distortion =                float2(0.0, 0.0);           // surfaceDescription.Distortion -- if distortion pass
				builtinData.distortionBlur =            0.0;                        // surfaceDescription.DistortionBlur -- if distortion pass
				builtinData.depthOffset =               0.0;                        // ApplyPerPixelDisplacement(input, V, layerTexCoord, blendMasks); #ifdef _DEPTHOFFSET_ON : ApplyDepthOffsetPositionInput(V, depthOffset, GetWorldToHClipMatrix(), posInput);
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				float mulTime32_g45 = _TimeParameters.x * ( AGW_WindGrassSpeed * _WindSpeed );
				float temp_output_9_0_g45 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
				float temp_output_1_0_g45 = inputMesh.ase_color.r;
				float temp_output_3_0_g45 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g45 ) ) * temp_output_9_0_g45 ) * temp_output_1_0_g45 ) + ( temp_output_9_0_g45 * temp_output_1_0_g45 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
				float3 appendResult70_g45 = (float3(temp_output_3_0_g45 , 0.0 , temp_output_3_0_g45));
				half3 Output_Wind159 = mul( GetWorldToObjectMatrix(), float4( ( appendResult70_g45 * AGW_WindDirection ) , 0.0 ) ).xyz;
				
				outputPackedVaryingsMeshToPS.ase_texcoord.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = Output_Wind159;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput
						#ifdef WRITE_NORMAL_BUFFER
						, out float4 outNormalBuffer : SV_Target0
							#ifdef WRITE_MSAA_DEPTH
							, out float1 depthColor : SV_Target1
							#endif
						#elif defined(WRITE_MSAA_DEPTH) // When only WRITE_MSAA_DEPTH is define and not WRITE_NORMAL_BUFFER it mean we are Unlit and only need depth, but we still have normal buffer binded
						, out float4 outNormalBuffer : SV_Target0
						, out float1 depthColor : SV_Target1
						#else
						, out float4 outColor : SV_Target0
						#endif

						#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
						#endif
						
					)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );	
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
				
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 V = float3(1.0, 1.0, 1.0);

				SurfaceData surfaceData;
				BuiltinData builtinData;
				AlphaSurfaceDescription surfaceDescription = ( AlphaSurfaceDescription ) 0;
				float2 uv_MainTex1 = packedInput.ase_texcoord.xy;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
				half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
				
				surfaceDescription.Alpha = Output_OpacityMask58;
				surfaceDescription.AlphaClipThreshold = _Cutoff;

				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif

				#ifdef WRITE_NORMAL_BUFFER
				EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);
				#ifdef WRITE_MSAA_DEPTH
				depthColor = packedInput.positionCS.z;
				#endif
				#elif defined(WRITE_MSAA_DEPTH)
				outNormalBuffer = float4(0.0, 0.0, 0.0, 1.0);
				depthColor = packedInput.positionCS.z;
				#elif defined(SCENESELECTIONPASS)
				outColor = float4(_ObjectId, _PassValue, 1.0, 1.0);
				#else
				outColor = float4(0.0, 0.0, 0.0, 0.0);
				#endif
			}
        
            ENDHLSL
        }

		
        Pass
        {
            
            
			Name "Forward"
			Tags { "LightMode"="Forward" }
			Stencil
			{
				Ref 2
				WriteMask 51
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}


            HLSLPROGRAM
            #define _MATERIAL_FEATURE_TRANSMISSION 1
            #define _NORMALMAP 1
            #define _ALPHATEST_ON 1
            #define ASE_SRP_VERSION 70105

            //#define UNITY_MATERIAL_LIT
			#pragma vertex Vert
			#pragma fragment Frag
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_FORWARD
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile DECALS_OFF DECALS_3RT DECALS_4RT
            #pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
			#pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH
				
			#define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
        
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
        
			#define HAS_LIGHTLOOP
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoop.hlsl"
        
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS

				
			struct AttributesMesh 
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				float3 interp00 : TEXCOORD0;
				float3 interp01 : TEXCOORD1;
				float4 interp02 : TEXCOORD2;
				float4 interp03 : TEXCOORD3;
				float4 interp04 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			half AGW_WindGrassScale;
			half AGW_WindGrassSpeed;
			half AGW_WindToggle;
			half AGW_WindGrassAmplitude;
			half AGW_WindGrassStiffness;
			half3 AGW_WindDirection;
			sampler2D _MainTex;
			sampler2D AG_TintNoiseTexture;
			half AG_TintNoiseTile;
			half AG_TintNoiseContrast;
			half AG_TintToggle;
			sampler2D _BumpMap;
			half AGG_SnowOffset;
			half AGG_SnowContrast;
			half AGG_SnowIntensity;
			half AGH_SnowMinimumHeight;
			half AGH_SnowFadeHeight;
			CBUFFER_START( UnityPerMaterial )
			half _WindScale;
			half _WindSpeed;
			half _WindAmplitude;
			half _WindGrassStiffness;
			half4 _BaseBottomColor;
			half4 _Color;
			half4 _TintColor1;
			half4 _TintColor2;
			half _TintNoiseTile;
			float4 _TopColor;
			half _TopOffset;
			half _TopContrast;
			half _TopIntensity;
			half _BaseBottomOffset;
			half _Glossiness;
			half _Cutoff;
			float _Thickness1;
			CBUFFER_END

				
			                
        
			void BuildSurfaceData ( FragInputs fragInputs, GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData )
			{
				ZERO_INITIALIZE ( SurfaceData, surfaceData );

				float3 normalTS = float3( 0.0f, 0.0f, 1.0f );
				normalTS = surfaceDescription.Normal;
				float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				GetNormalWS ( fragInputs, normalTS, surfaceData.normalWS ,doubleSidedConstants);

				surfaceData.ambientOcclusion = 1.0f;

				surfaceData.baseColor = surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness = surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion = surfaceDescription.Occlusion;

				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				surfaceData.specularColor = surfaceDescription.Specular;
				#else
				surfaceData.metallic = surfaceDescription.Metallic;
				#endif

				#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceData.diffusionProfileHash = asuint(surfaceDescription.DiffusionProfile);
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				surfaceData.subsurfaceMask = surfaceDescription.SubsurfaceMask;
				#else
				surfaceData.subsurfaceMask = 1.0f;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				surfaceData.thickness = surfaceDescription.Thickness;
				#endif

				surfaceData.tangentWS = normalize ( fragInputs.tangentToWorld[ 0 ].xyz );
				surfaceData.tangentWS = Orthonormalize ( surfaceData.tangentWS, surfaceData.normalWS );

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				surfaceData.anisotropy = surfaceDescription.Anisotropy;

				#else
				surfaceData.anisotropy = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				surfaceData.coatMask = surfaceDescription.CoatMask;
				#else
				surfaceData.coatMask = 0.0f;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				surfaceData.iridescenceThickness = surfaceDescription.IridescenceThickness;
				surfaceData.iridescenceMask = surfaceDescription.IridescenceMask;
				#else
				surfaceData.iridescenceThickness = 0.0;
				surfaceData.iridescenceMask = 1.0;
				#endif

				//ASE CUSTOM TAG
				#ifdef _MATERIAL_FEATURE_TRANSPARENCY
				surfaceData.ior = surfaceDescription.IndexOfRefraction;
				surfaceData.transmittanceColor = surfaceDescription.TransmittanceColor;
				surfaceData.atDistance = surfaceDescription.TransmittanceAbsorptionDistance;
				surfaceData.transmittanceMask = surfaceDescription.TransmittanceMask;
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1000000.0;
				surfaceData.transmittanceMask = 0.0;
				#endif

				surfaceData.specularOcclusion = 1.0;

				#if defined(_BENTNORMALMAP) && defined(_ENABLESPECULAROCCLUSION)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO ( V, bentNormalWS, surfaceData );
				#elif defined(_MASKMAP)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion ( NdotV, surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness ( surfaceData.perceptualSmoothness ) );
				#endif
				#if HAVE_DECALS
				if (_EnableDecals)
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData (posInput, surfaceDescription.Alpha);
					ApplyDecalToSurfaceData (decalSurfaceData, surfaceData);
				}
				#endif
			}
        
			void GetSurfaceAndBuiltinData( GlobalSurfaceDescription surfaceDescription , FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#if _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
		
				BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
				InitBuiltinData (posInput, surfaceDescription.Alpha, surfaceData.normalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);
        
				builtinData.emissiveColor =             surfaceDescription.Emission;
				builtinData.distortion =                float2(0.0, 0.0);           // surfaceDescription.Distortion -- if distortion pass
				builtinData.distortionBlur =            0.0;                        // surfaceDescription.DistortionBlur -- if distortion pass
        
				builtinData.depthOffset =               0.0;                        // ApplyPerPixelDisplacement(input, V, layerTexCoord, blendMasks); #ifdef _DEPTHOFFSET_ON : ApplyDepthOffsetPositionInput(V, depthOffset, GetWorldToHClipMatrix(), posInput);
        
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}
        
			
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				float mulTime32_g45 = _TimeParameters.x * ( AGW_WindGrassSpeed * _WindSpeed );
				float temp_output_9_0_g45 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
				float temp_output_1_0_g45 = inputMesh.ase_color.r;
				float temp_output_3_0_g45 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g45 ) ) * temp_output_9_0_g45 ) * temp_output_1_0_g45 ) + ( temp_output_9_0_g45 * temp_output_1_0_g45 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
				float3 appendResult70_g45 = (float3(temp_output_3_0_g45 , 0.0 , temp_output_3_0_g45));
				half3 Output_Wind159 = mul( GetWorldToObjectMatrix(), float4( ( appendResult70_g45 * AGW_WindDirection ) , 0.0 ) ).xyz;
				
				float4 temp_cast_2 = (1.0).xxxx;
				float2 appendResult8_g44 = (float2(ase_worldPos.x , ase_worldPos.z));
				float4 lerpResult17_g44 = lerp( _TintColor1 , _TintColor2 , saturate( ( tex2Dlod( AG_TintNoiseTexture, float4( ( appendResult8_g44 * ( 0.001 * AG_TintNoiseTile * _TintNoiseTile ) ), 0, 0.0) ).r * (0.001 + (AG_TintNoiseContrast - 0.001) * (60.0 - 0.001) / (10.0 - 0.001)) ) ));
				float4 lerpResult19_g44 = lerp( temp_cast_2 , lerpResult17_g44 , AG_TintToggle);
				float4 vertexToFrag18_g44 = lerpResult19_g44;
				outputPackedVaryingsMeshToPS.ase_texcoord6 = vertexToFrag18_g44;
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				outputPackedVaryingsMeshToPS.ase_texcoord7.xyz = ase_worldBitangent;
				
				outputPackedVaryingsMeshToPS.ase_texcoord5.xy = inputMesh.ase_texcoord.xy;
				outputPackedVaryingsMeshToPS.ase_color = inputMesh.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord5.zw = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = Output_Wind159;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif
					
				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.interp00.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.interp01.xyz = normalWS;
				outputPackedVaryingsMeshToPS.interp02.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.interp03.xyzw = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.interp04.xyzw = inputMesh.uv2;
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(	PackedVaryingsMeshToPS packedInput,
						#ifdef OUTPUT_SPLIT_LIGHTING
						out float4 outColor : SV_Target0, 
						out float4 outDiffuseLighting : SV_Target1,
						OUTPUT_SSSBUFFER (outSSSBuffer)
						#else
						out float4 outColor : SV_Target0
						#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
						, out float4 outMotionVec : SV_Target1
						#endif 
						#endif 
						#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
						#endif
						, FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC 
						)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
        
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
				float3 positionRWS = packedInput.interp00.xyz;
				float3 normalWS = packedInput.interp01.xyz;
				float4 tangentWS = packedInput.interp02.xyzw;
						
				input.positionRWS = positionRWS;
				input.tangentToWorld = BuildTangentToWorld(tangentWS, normalWS);
				input.texCoord1 = packedInput.interp03.xyzw;
				input.texCoord2 = packedInput.interp04.xyzw;

				// input.positionSS is SV_Position
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(input.positionSS.xy) / GetTileSize() );

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir ( input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GlobalSurfaceDescription surfaceDescription = ( GlobalSurfaceDescription ) 0;
				float2 uv_MainTex1 = packedInput.ase_texcoord5.xy;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
				float4 vertexToFrag18_g44 = packedInput.ase_texcoord6;
				half4 TintColor229 = vertexToFrag18_g44;
				float2 uv_BumpMap23 = packedInput.ase_texcoord5.xy;
				float3 break105_g36 = UnpackNormalmapRGorAG( tex2D( _BumpMap, uv_BumpMap23 ), 1.0f );
				float switchResult107_g36 = (((ase_vface>0)?(break105_g36.z):(-break105_g36.z)));
				float3 appendResult108_g36 = (float3(break105_g36.x , break105_g36.y , switchResult107_g36));
				half3 Output_Normal96 = appendResult108_g36;
				float3 desaturateInitialColor301 = Output_Normal96;
				float desaturateDot301 = dot( desaturateInitialColor301, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar301 = lerp( desaturateInitialColor301, desaturateDot301.xxx, 1.0 );
				float3 ase_worldBitangent = packedInput.ase_texcoord7.xyz;
				float3 tanToWorld0 = float3( tangentWS.xyz.x, ase_worldBitangent.x, normalWS.x );
				float3 tanToWorld1 = float3( tangentWS.xyz.y, ase_worldBitangent.y, normalWS.y );
				float3 tanToWorld2 = float3( tangentWS.xyz.z, ase_worldBitangent.z, normalWS.z );
				float3 tanNormal218 = desaturateVar301;
				float3 worldNormal218 = float3(dot(tanToWorld0,tanNormal218), dot(tanToWorld1,tanNormal218), dot(tanToWorld2,tanNormal218));
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				half TopMask211 = ( saturate( ( pow( abs( ( saturate( worldNormal218.y ) + ( _TopOffset * AGG_SnowOffset ) ) ) , ( (1.0 + (_TopContrast - 0.0) * (200.0 - 1.0) / (1.0 - 0.0)) * AGG_SnowContrast ) ) * ( _TopIntensity * AGG_SnowIntensity ) ) ) * saturate( (0.0 + (ase_worldPos.y - AGH_SnowMinimumHeight) * (1.0 - 0.0) / (( AGH_SnowMinimumHeight + AGH_SnowFadeHeight ) - AGH_SnowMinimumHeight)) ) );
				float4 lerpResult215 = lerp( ( _Color * tex2DNode1 * TintColor229 ) , _TopColor , TopMask211);
				float4 lerpResult8 = lerp( ( _BaseBottomColor * lerpResult215 ) , lerpResult215 , saturate( pow( abs( ( packedInput.ase_color.r * 2.0 ) ) , _BaseBottomOffset ) ));
				half4 Output_Albedo97 = lerpResult8;
				
				half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
				
				surfaceDescription.Albedo = Output_Albedo97.rgb;
				surfaceDescription.Normal = Output_Normal96;
				surfaceDescription.Emission = 0;
				surfaceDescription.Specular = 0;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Smoothness = _Glossiness;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = Output_OpacityMask58;
				surfaceDescription.AlphaClipThreshold = _Cutoff;

				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
				surfaceDescription.CoatMask = 0;
				#endif

				#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.DiffusionProfile = 2.0088424682617188;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = 1;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceDescription.Thickness = _Thickness1;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceThickness = 0;
				surfaceDescription.IridescenceMask = 1;
				#endif

				#ifdef _MATERIAL_FEATURE_TRANSPARENCY
				surfaceDescription.IndexOfRefraction = 1;
				surfaceDescription.TransmittanceColor = float3( 1, 1, 1 );
				surfaceDescription.TransmittanceAbsorptionDistance = 1000000;
				surfaceDescription.TransmittanceMask = 0;
				#endif
				GetSurfaceAndBuiltinData(surfaceDescription, input, normalizedWorldViewDir, posInput, surfaceData, builtinData);

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData(input.positionSS.xy, surfaceData);

				PreLightData preLightData = GetPreLightData(normalizedWorldViewDir, posInput, bsdfData);

				outColor = float4(0.0, 0.0, 0.0, 0.0);

				{
					#ifdef _SURFACE_TYPE_TRANSPARENT
					uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_TRANSPARENT;
					#else
					uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;
					#endif
					float3 diffuseLighting;
					float3 specularLighting;

					//LightLoop(normalizedWorldViewDir, posInput, preLightData, bsdfData, builtinData, featureFlags, diffuseLighting, specularLighting);
						
					//diffuseLighting *= GetCurrentExposureMultiplier();
					//specularLighting *= GetCurrentExposureMultiplier();

					#ifdef OUTPUT_SPLIT_LIGHTING
					if (_EnableSubsurfaceScattering != 0 && ShouldOutputSplitLighting(bsdfData))
					{
						outColor = float4(specularLighting, 1.0);
						outDiffuseLighting = float4(TagLightingForSSS(diffuseLighting), 1.0);
					}
					else
					{
						outColor = float4(diffuseLighting + specularLighting, 1.0);
						outDiffuseLighting = 0;
					}
					ENCODE_INTO_SSSBUFFER(surfaceData, posInput.positionSS, outSSSBuffer);
					#else
					//outColor = ApplyBlendMode(diffuseLighting, specularLighting, builtinData.opacity);
					outColor = EvaluateAtmosphericScattering(posInput, normalizedWorldViewDir, outColor);
					#endif
					#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					//VaryingsPassToPS inputPass = UnpackVaryingsPassToPS (packedInput.vpass);
					//bool forceNoMotion = any (unity_MotionVectorsParams.yw == 0.0);
					//if (forceNoMotion)
					//{
					//	outMotionVec = float4(2.0, 0.0, 0.0, 0.0);
					//}
					//else
					//{
					//	float2 motionVec = CalculateMotionVector (inputPass.positionCS, inputPass.previousPositionCS);
					//	EncodeMotionVector (motionVec * 0.5, outMotionVec);
					//	outMotionVec.zw = 1.0;
					//}
					#endif
				}

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif
			}

            ENDHLSL
        }
		
    }
    Fallback "Hidden/InternalErrorShader"
	
	
}
/*ASEBEGIN
Version=17704
1920;0;1920;1019;-1027.562;1171.183;1;True;False
Node;AmplifyShaderEditor.SamplerNode;23;256,-1024;Inherit;True;Property;_BumpMap;Normal Map;8;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;309;576,-1024;Inherit;False;AG Flip Normals;-1;;36;02ae90bb716acd647b8ac9db8603316a;0;1;110;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;832,-1024;Half;False;Output_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;300;-1792,608;Half;False;Constant;_Float1;Float 1;22;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;299;-1792,512;Inherit;False;96;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;204;-1664,896;Half;False;Property;_TopContrast;Top Contrast;11;0;Create;True;0;0;False;0;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;301;-1536,512;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;207;-1280,768;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;200;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;218;-1280,512;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;208;-1664,800;Half;False;Property;_TopOffset;Top Offset;10;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;224;256,512;Half;False;Property;_TintColor1;Tint Color 1;13;0;Create;True;0;0;False;1;Header(Tint Color);1,1,1,0;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;225;256,704;Half;False;Property;_TintColor2;Tint Color 2;14;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;226;256,896;Half;False;Property;_TintNoiseTile;Tint Noise Tile;15;0;Create;True;0;0;False;0;10;10;0.001;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-1664,704;Half;False;Property;_TopIntensity;Top Intensity;9;0;Create;True;0;0;False;1;Header(Top);1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;298;-896,512;Inherit;False;AG Global Snow - Grass;-1;;42;ac8896bffa708e24e91ea416ba6ae274;0;4;22;FLOAT;0;False;24;FLOAT;0;False;23;FLOAT;0;False;25;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;313;-896,672;Inherit;False;AG Global Snow - Height;-1;;43;792d553d9b3743f498843a42559debdb;0;0;1;FLOAT;43
Node;AmplifyShaderEditor.FunctionNode;295;640,512;Inherit;False;AG Global Tint Color;0;;44;1dcc860732522ee469468f952b4e8aa1;0;3;27;COLOR;0,0,0,0;False;28;COLOR;0,0,0,0;False;22;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;314;-512,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;229;1024,512;Half;False;TintColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;150;-1792,-160;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;156;-1792,32;Half;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;230;-1792,-432;Inherit;False;229;TintColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;245;-1792,2016;Half;False;Property;_WindGrassStiffness;Wind Grass Stiffness;20;0;Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-1408,-160;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1792,-640;Inherit;True;Property;_MainTex;Albedo (A Opacity);7;1;[NoScaleOffset];Create;False;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;32;-1792,-832;Half;False;Property;_Color;Base Color;4;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;211;-256,512;Half;False;TopMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;260;-1408,1920;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;5;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-1408,-384;Inherit;False;211;TopMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1408,-640;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-1792,1824;Half;False;Property;_WindScale;Wind Scale;19;0;Create;True;0;0;False;0;10;12.8;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;213;-1792,-352;Float;False;Property;_TopColor;Top Color;12;0;Create;True;0;0;False;0;1,1,1,0;0.7279412,0.7279412,0.7279412,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;149;-1792,1536;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;147;-1792,1920;Half;False;Property;_WindSpeed;Wind Speed;18;0;Create;True;0;0;False;0;5;7.17;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;296;-1152,-160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1408,-32;Half;False;Property;_BaseBottomOffset;Base Bottom Offset;6;0;Create;True;0;0;False;0;5;0.38;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-1792,1728;Half;False;Property;_WindAmplitude;Wind Amplitude;17;0;Create;True;0;0;False;1;Header(Wind);1;0.62;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;241;-1152,1728;Half;False;Global;AGW_WindDirection;AGW_WindDirection;20;0;Create;True;0;0;False;0;0,0,0;0.9382213,-0.3195158,-0.1328554;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;154;-1024,-160;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;215;-1152,-640;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-1792,-1024;Half;False;Property;_BaseBottomColor;Base Bottom Color;5;0;Create;True;0;0;False;0;1,1,1,0;0.5808823,0.5808823,0.5808823,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;297;-1152,1536;Inherit;False;AG Global Wind - Grass;-1;;45;ab99ce4e769429241ac615381e930fc9;0;5;1;FLOAT;0;False;54;FLOAT;0;False;48;FLOAT;0;False;41;FLOAT;0;False;55;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-896,-1024;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;13;-768,-160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-768,1536;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldToObjectMatrix;243;-768,1664;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1408,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;-640,-1024;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-512,1536;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-256,1536;Half;False;Output_Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-1152,-768;Half;False;Output_OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-384,-1024;Half;False;Output_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;217;1664,-640;Half;False;Property;_Cutoff;Alpha Cutoff;2;0;Create;False;0;0;True;0;0.5;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;316;1664,-448;Inherit;False;Property;_Thickness1;Translucency Thickness;16;0;Create;False;0;0;False;1;Header(Translucency);0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;160;1664,-352;Inherit;False;159;Output_Wind;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;1664,-736;Inherit;False;58;Output_OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;1664,-928;Inherit;False;96;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;134;1664,-832;Half;False;Property;_Glossiness;Base Smoothness;3;0;Create;False;0;0;False;1;Header(Base);0;0.466;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;1664,-1024;Inherit;False;97;Output_Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DiffusionProfileNode;315;1664,-544;Float;False;Constant;_DiffusionProfile;_DiffusionProfile;21;0;Create;True;0;0;False;0;86d97fbd3ca825843866769b15956999;e757c67f815166a46bf02923652e8e19;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;307;2176,-1024;Float;False;False;-1;2;ASEMaterialInspector;0;4;Hidden/Templates/HDSRPPBR;bb308bce79762c34e823049efce65141;True;Motion Vectors;0;5;Motion Vectors;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;False;False;True;True;128;False;-1;255;False;-1;176;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;True;1;LightMode=MotionVectors;False;0;Hidden/InternalErrorShader;0;0;Translucent;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;308;2176,-1024;Float;False;False;-1;2;ASEMaterialInspector;0;4;Hidden/Templates/HDSRPPBR;bb308bce79762c34e823049efce65141;True;Forward;0;6;Forward;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;False;False;True;True;2;False;-1;255;False;-1;51;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;True;1;LightMode=Forward;False;0;Hidden/InternalErrorShader;0;0;Translucent;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;306;2176,-1024;Float;False;False;-1;2;ASEMaterialInspector;0;4;Hidden/Templates/HDSRPPBR;bb308bce79762c34e823049efce65141;True;DepthOnly;0;4;DepthOnly;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;False;False;True;True;0;False;-1;255;False;-1;48;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Translucent;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;302;2176,-1024;Float;False;True;-1;2;;0;4;ANGRYMESH/Nature Pack/HDRP/Grass Snow;bb308bce79762c34e823049efce65141;True;GBuffer;0;0;GBuffer;22;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;False;False;True;True;2;False;-1;255;False;-1;51;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;True;1;LightMode=GBuffer;False;0;Hidden/InternalErrorShader;0;0;Translucent;1;Vertex Position,InvertActionOnDeselection;1;0;7;True;True;True;True;True;False;True;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;305;2176,-1024;Float;False;False;-1;2;ASEMaterialInspector;0;4;Hidden/Templates/HDSRPPBR;bb308bce79762c34e823049efce65141;True;SceneSelectionPass;0;3;SceneSelectionPass;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;0;Hidden/InternalErrorShader;0;0;Translucent;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;303;2176,-1024;Float;False;False;-1;2;ASEMaterialInspector;0;4;Hidden/Templates/HDSRPPBR;bb308bce79762c34e823049efce65141;True;META;0;1;META;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Translucent;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;304;2176,-1024;Float;False;False;-1;2;ASEMaterialInspector;0;4;Hidden/Templates/HDSRPPBR;bb308bce79762c34e823049efce65141;True;ShadowCaster;0;2;ShadowCaster;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;False;False;True;1;False;-1;True;3;False;-1;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Translucent;0;0
Node;AmplifyShaderEditor.CommentaryNode;60;-1792,-1152;Inherit;False;1662.288;100;;0;// Albedo Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;59;256,-1152;Inherit;False;767.1542;100;;0;// Normal Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;189;-1792,1408;Inherit;False;1793.364;100;;0;// Vertex Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;279;1664,-1152;Inherit;False;767.1542;100;;0;// Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;257;256,384;Inherit;False;1023;100;;0;// Tint Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;258;-1792,384;Inherit;False;1791.944;100;;0;// Top World Mapping;1,1,1,1;0;0
WireConnection;309;110;23;0
WireConnection;96;0;309;0
WireConnection;301;0;299;0
WireConnection;301;1;300;0
WireConnection;207;0;204;0
WireConnection;218;0;301;0
WireConnection;298;22;218;2
WireConnection;298;24;206;0
WireConnection;298;23;208;0
WireConnection;298;25;207;0
WireConnection;295;27;224;0
WireConnection;295;28;225;0
WireConnection;295;22;226;0
WireConnection;314;0;298;0
WireConnection;314;1;313;43
WireConnection;229;0;295;0
WireConnection;155;0;150;1
WireConnection;155;1;156;0
WireConnection;211;0;314;0
WireConnection;260;0;245;0
WireConnection;33;0;32;0
WireConnection;33;1;1;0
WireConnection;33;2;230;0
WireConnection;296;0;155;0
WireConnection;154;0;296;0
WireConnection;154;1;11;0
WireConnection;215;0;33;0
WireConnection;215;1;213;0
WireConnection;215;2;214;0
WireConnection;297;1;149;1
WireConnection;297;54;148;0
WireConnection;297;48;176;0
WireConnection;297;41;147;0
WireConnection;297;55;260;0
WireConnection;34;0;6;0
WireConnection;34;1;215;0
WireConnection;13;0;154;0
WireConnection;242;0;297;0
WireConnection;242;1;241;0
WireConnection;99;0;32;4
WireConnection;99;1;1;4
WireConnection;8;0;34;0
WireConnection;8;1;215;0
WireConnection;8;2;13;0
WireConnection;244;0;243;0
WireConnection;244;1;242;0
WireConnection;159;0;244;0
WireConnection;58;0;99;0
WireConnection;97;0;8;0
WireConnection;302;0;157;0
WireConnection;302;1;158;0
WireConnection;302;5;134;0
WireConnection;302;7;57;0
WireConnection;302;8;217;0
WireConnection;302;12;315;0
WireConnection;302;14;316;0
WireConnection;302;9;160;0
ASEEND*/
//CHKSM=86582C7BE5B2974646B78F61FC75F7D302AAD13D