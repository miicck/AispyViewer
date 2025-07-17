Shader "AtomShader"
{
    Properties
    {
        _AtomScale("Atom scale", float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _AtomScale;
            
            struct mesh_data
            {
                float4 color;
                float4 position;
            };

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 normal: NORMAL;
                float4 color : COLOR;
            };

            struct v2_f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            StructuredBuffer<mesh_data> data;

            v2_f vert(const appdata_t i, const uint instance_id: SV_InstanceID)
            {
                v2_f o;

                static const float4 lightNormal = float4(0.5773, 0.5773, 0.5773, 0);
                const mesh_data m_data = data[instance_id];
                const float4 pos = m_data.position + i.vertex*_AtomScale;
                const float brightness = clamp(dot(i.normal, lightNormal), 0, 0.9) + 0.1;
                o.vertex = UnityObjectToClipPos(pos);
                o.color = m_data.color * brightness;

                return o;
            }

            fixed4 frag(v2_f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}