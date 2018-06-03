// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|emission-2856-OUT,alpha-2856-OUT;n:type:ShaderForge.SFN_TexCoord,id:8056,x:30961,y:32998,varname:node_8056,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Distance,id:514,x:31145,y:33056,varname:node_514,prsc:2|A-8056-UVOUT,B-6233-OUT;n:type:ShaderForge.SFN_Vector2,id:6233,x:30961,y:33168,varname:node_6233,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Clamp,id:7704,x:31338,y:33072,varname:node_7704,prsc:2|IN-514-OUT,MIN-6440-OUT,MAX-1571-OUT;n:type:ShaderForge.SFN_Vector1,id:6440,x:31145,y:33195,varname:node_6440,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:1571,x:31145,y:33253,varname:node_1571,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:2217,x:31511,y:32939,varname:node_2217,prsc:2|A-8051-OUT,B-7704-OUT;n:type:ShaderForge.SFN_Vector1,id:8051,x:31338,y:32996,varname:node_8051,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:7991,x:31511,y:33092,varname:node_7991,prsc:2,v1:2;n:type:ShaderForge.SFN_TexCoord,id:9860,x:31145,y:33332,varname:node_9860,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Clamp,id:3113,x:31329,y:33476,varname:node_3113,prsc:2|IN-9860-UVOUT,MIN-2689-OUT,MAX-6556-OUT;n:type:ShaderForge.SFN_Vector2,id:2689,x:31145,y:33494,varname:node_2689,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Vector2,id:6556,x:31145,y:33592,varname:node_6556,prsc:2,v1:1,v2:1;n:type:ShaderForge.SFN_Subtract,id:7373,x:31533,y:33476,varname:node_7373,prsc:2|A-3113-OUT,B-2689-OUT;n:type:ShaderForge.SFN_Multiply,id:7809,x:31709,y:33498,varname:node_7809,prsc:2|A-7373-OUT,B-8528-OUT;n:type:ShaderForge.SFN_Vector1,id:8528,x:31496,y:33625,varname:node_8528,prsc:2,v1:5;n:type:ShaderForge.SFN_Length,id:2878,x:31894,y:33487,varname:node_2878,prsc:2|IN-7809-OUT;n:type:ShaderForge.SFN_Multiply,id:2856,x:31815,y:33026,varname:node_2856,prsc:2|A-2217-OUT,B-7991-OUT,C-7224-OUT;n:type:ShaderForge.SFN_OneMinus,id:7224,x:31829,y:33263,varname:node_7224,prsc:2|IN-2878-OUT;pass:END;sub:END;*/

Shader "Shader Forge/Warp Shader" {
    Properties {
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_2217 = (0.5-clamp(distance(i.uv0,float2(0.5,0.5)),0.0,0.5));
                float node_7991 = 2.0;
                float2 node_2689 = float2(0.5,0.5);
                float2 node_3113 = clamp(i.uv0,node_2689,float2(1,1));
                float node_2856 = (node_2217*node_7991*(1.0 - length(((node_3113-node_2689)*5.0))));
                float3 emissive = float3(node_2856,node_2856,node_2856);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_2856);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
