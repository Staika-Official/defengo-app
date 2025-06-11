// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HalfToneTansition"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DotNumber("Dot Number", Float) = 5
		_DotScale("Dot Scale", Float) = 0.37
		_X_directionToggleLeft("X_directionToggleLeft", Float) = 0
		_X_directionToggleRight("X_directionToggleRight", Float) = 1
		_bgColorTop("bgColorTop", Color) = (1,1,1,0)
		_bgColorBottom("bgColorBottom", Color) = (1,1,1,0)
		_mixValue("mixValue", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float _X_directionToggleLeft;
		uniform float _X_directionToggleRight;
		uniform float _DotNumber;
		uniform float _DotScale;
		uniform float4 _bgColorTop;
		uniform float4 _bgColorBottom;
		uniform float _mixValue;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 temp_cast_0 = (0.5).xx;
			float2 temp_output_58_0 = ( frac( ( i.uv_texcoord * _DotNumber ) ) - temp_cast_0 );
			float2 temp_cast_1 = (0.5).xx;
			float dotResult56 = dot( temp_output_58_0 , temp_output_58_0 );
			float temp_output_63_0 = ( 1.0 - abs( pow( dotResult56 , _DotScale ) ) );
			float temp_output_65_0 = step( ase_screenPosNorm.x , temp_output_63_0 );
			float temp_output_134_0 = pow( ( 1.0 - ase_screenPosNorm.y ) , 2.0 );
			float4 lerpResult139 = lerp( ( temp_output_65_0 * _bgColorTop ) , ( _bgColorBottom * temp_output_134_0 ) , _mixValue);
			float temp_output_108_0 = ( 1.0 - step( temp_output_63_0 , ( 1.0 - ase_screenPosNorm.x ) ) );
			float4 lerpResult131 = lerp( ( temp_output_108_0 * _bgColorTop ) , ( _bgColorBottom * temp_output_134_0 ) , _mixValue);
			float4 ifLocalVar112 = 0;
			if( _X_directionToggleLeft > _X_directionToggleRight )
				ifLocalVar112 = lerpResult139;
			else if( _X_directionToggleLeft < _X_directionToggleRight )
				ifLocalVar112 = lerpResult131;
			o.Emission = ifLocalVar112.rgb;
			float ifLocalVar75 = 0;
			if( _X_directionToggleLeft > _X_directionToggleRight )
				ifLocalVar75 = temp_output_65_0;
			else if( _X_directionToggleLeft < _X_directionToggleRight )
				ifLocalVar75 = temp_output_108_0;
			o.Alpha = ifLocalVar75;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
1420;187;1609;912;8840.77;3553.707;6.255099;True;True
Node;AmplifyShaderEditor.CommentaryNode;135;-6391.47,-2083.052;Inherit;False;2800.201;851.6885;Comment;16;57;62;61;59;60;58;56;66;104;64;63;105;73;65;108;144;Dot Pattern;1,0,0,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;57;-6080.878,-2033.052;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;62;-5959.435,-1630.433;Inherit;True;Property;_DotNumber;Dot Number;0;0;Create;True;0;0;False;0;False;5;110;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-5700.968,-1894.611;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;60;-5432.201,-1875.683;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-5481.83,-1575.867;Inherit;True;Constant;_Float2;Float 2;1;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;58;-5201.255,-2000.507;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;56;-4963.246,-2004.693;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-5095.33,-1670.299;Inherit;True;Property;_DotScale;Dot Scale;1;0;Create;True;0;0;False;0;False;0.37;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;64;-4686.104,-1930.327;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;144;-4422.081,-1880.476;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;104;-4847.258,-1483.515;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;137;-4428.776,-410.2401;Inherit;False;1490.672;581.406;Comment;5;113;133;114;134;115;Bottom Color;1,0.9238702,0,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;105;-4411.431,-1531.449;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;113;-4403.462,-346.1273;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;63;-4139.694,-1918.016;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-4459.125,-1150.466;Inherit;False;845.3108;617.4271;Comment;3;109;110;111;Top Color;0,0.8937981,1,1;0;0
Node;AmplifyShaderEditor.StepOpNode;73;-4179.577,-1529.771;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.46;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;133;-4186.423,-107.6443;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;108;-3890.156,-1527.189;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;114;-3980.197,-352.0502;Inherit;False;Property;_bgColorBottom;bgColorBottom;5;0;Create;True;0;0;False;0;False;1,1,1,0;0.1368543,0,0.9058824,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;134;-3851.144,-94.05553;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;109;-4409.123,-952.1285;Inherit;False;Property;_bgColorTop;bgColorTop;4;0;Create;True;0;0;False;0;False;1,1,1,0;0.2313724,0.8162314,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;65;-3826.273,-1858.529;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.46;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-3307.07,-180.7795;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-3884.342,-1078.92;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-3196.268,-884.5713;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-2753.203,-309.2563;Inherit;False;Property;_mixValue;mixValue;6;0;Create;True;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-3848.815,-787.0386;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2357.345,-2103.47;Float;False;Property;_X_directionToggleLeft;X_directionToggleLeft;2;0;Create;True;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;131;-2534.471,-649.0529;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;139;-2558.302,-1097.02;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-2398.224,-1945.846;Float;False;Property;_X_directionToggleRight;X_directionToggleRight;3;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;75;-1453.846,-1830.004;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;112;-1920.907,-1224.455;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;95;-812.8724,-1544.84;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;HalfToneTansition;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;61;0;57;0
WireConnection;61;1;62;0
WireConnection;60;0;61;0
WireConnection;58;0;60;0
WireConnection;58;1;59;0
WireConnection;56;0;58;0
WireConnection;56;1;58;0
WireConnection;64;0;56;0
WireConnection;64;1;66;0
WireConnection;144;0;64;0
WireConnection;105;0;104;1
WireConnection;63;0;144;0
WireConnection;73;0;63;0
WireConnection;73;1;105;0
WireConnection;133;0;113;2
WireConnection;108;0;73;0
WireConnection;134;0;133;0
WireConnection;65;0;104;1
WireConnection;65;1;63;0
WireConnection;115;0;114;0
WireConnection;115;1;134;0
WireConnection;111;0;65;0
WireConnection;111;1;109;0
WireConnection;125;0;114;0
WireConnection;125;1;134;0
WireConnection;110;0;108;0
WireConnection;110;1;109;0
WireConnection;131;0;110;0
WireConnection;131;1;115;0
WireConnection;131;2;132;0
WireConnection;139;0;111;0
WireConnection;139;1;125;0
WireConnection;139;2;132;0
WireConnection;75;0;76;0
WireConnection;75;1;79;0
WireConnection;75;2;65;0
WireConnection;75;4;108;0
WireConnection;112;0;76;0
WireConnection;112;1;79;0
WireConnection;112;2;139;0
WireConnection;112;4;131;0
WireConnection;95;2;112;0
WireConnection;95;9;75;0
ASEEND*/
//CHKSM=BDD782E1FED4F264C88815166E4E83BDEA5B2C72