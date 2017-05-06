using UnityEngine;
using System.Collections;

public class CarLights : MonoBehaviour {

    public Light m_headlight1;
    public Light m_headlight2;
    public Light m_headlight3;
    public Light m_breaklight1;
    public Light m_breaklight2;
    public Light m_reverselight1;
    public Light m_reverselight2;
    public Light m_hazardlight1;
    public Light m_hazardlight2;
    public Light m_taillight1;
    public Light m_taillight2;

    public Renderer m_headlights;
    public Renderer m_breaklights;
    public Renderer m_reverselights;
    public Renderer m_hazardlightsleft;
    public Renderer m_hazardlightsright;
    public Renderer m_backlights;
    public Renderer m_frontlights;
    public Renderer m_foglights;
    public Renderer m_taillights;

    public Material m_headlightsOFF;
    public Material m_headlightsON;
    public Material m_reverselightsOFF;
    public Material m_reverselightsON;
    public Material m_breaklightsOFF;
    public Material m_breaklightsON;
    public Material m_hazardsOFF;
    public Material m_hazardsON;
    public Material m_taillightsOFF;
    public Material m_taillightsON;

    bool m_insideTunnel;
    bool m_hazardsOn = false;

    public enum m_LightType
    {
        Headlight,
        Breaklight,
        Reverselight,
        Hazardlight
    }

    public enum m_CarType
    {
        MonteCarlo,
        Manta,
        Mini,
        JXJ220,
        IceCreamVan,
        MGMetro,
        RV,
        OpelGT,
        LotusCarlton,
        ZUK,
        Capri
    }

    public m_CarType m_myCarType;


    float m_materialLowerValue;
    float m_materialUpperValue;
    float m_emission;

    Kojima.CarScript carscript;

    RaycastHit m_hit;

    void Start()
    {

        m_insideTunnel = false;

        carscript = gameObject.GetComponent<Kojima.CarScript>();
        
        if (m_myCarType == m_CarType.JXJ220)
        {         
            m_taillight1.intensity = 0;
            m_taillight2.intensity = 0;
        }

        if (m_myCarType == m_CarType.Manta || m_myCarType == m_CarType.MGMetro)
        {
            m_headlight3.intensity = 0;
        }

        m_headlight1.intensity = 0;
        m_headlight2.intensity = 0;
        m_breaklight1.intensity = 0;
        m_breaklight2.intensity = 0;
        m_hazardlight1.intensity = 0;
        m_hazardlight2.intensity = 0;
        if (m_myCarType != m_CarType.RV && m_myCarType != m_CarType.IceCreamVan)
        {
            m_reverselight1.intensity = 0;
            if (m_myCarType != m_CarType.OpelGT)
            {
                m_reverselight2.intensity = 0;
            }
        }      
        

    }

    void Update()
    {
        //Use the Mathf.pingpong function to blink the hazard lights
        if(carscript.UpsideDown)
        {
            if (m_myCarType == m_CarType.LotusCarlton)
            {
                Material[] alternativeMaterials = m_backlights.materials;
                alternativeMaterials[2] = m_hazardsON;
                m_backlights.materials = alternativeMaterials;
                Material[] alternativeMaterials1 = m_frontlights.materials;
                alternativeMaterials1[1] = m_hazardsON;
                m_frontlights.materials = alternativeMaterials1;
                m_materialLowerValue = 0f;
                m_materialUpperValue = 1f;
                m_emission = m_materialLowerValue + Mathf.PingPong(Time.time * 2f, m_materialUpperValue - m_materialLowerValue);

                m_backlights.materials[2].SetColor("_EmissionColor", new Color(1f, 1f, 1f) * m_emission);
                m_frontlights.materials[1].SetColor("_EmissionColor", new Color(1f, 1f, 1f) * m_emission);
            }
            else
            {
                if (m_hazardlightsleft && m_hazardlightsright)
                {
                    m_hazardlightsleft.material = m_hazardsON;
                    m_hazardlightsright.material = m_hazardsON;
                }
                
                m_materialLowerValue = 0f;
                m_materialUpperValue = 1f;
                m_emission = m_materialLowerValue + Mathf.PingPong(Time.time * 2f, m_materialUpperValue - m_materialLowerValue);

                if (m_myCarType != m_CarType.Capri)
                {
                    m_hazardlightsleft.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * m_emission);
                    m_hazardlightsright.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * m_emission);
                }
                
            }
            
        }
        else
        {
            if (m_insideTunnel == false || DayNightCycleScript.s_singleton.IsDay())
            {
                if (m_hazardlight1.intensity > 0)
                {
                    DeactivateLights(m_LightType.Hazardlight);
                }
            }
        }
        
        //Turn the breaklights on if the car is breaking
        if (carscript.CurrentlyBraking == true)
        {
            ActivateLights(m_LightType.Breaklight, 3);
        }
        else
        {
            //Only turn the lights off if they aren't being toggled by a different event
            if (m_insideTunnel == false || DayNightCycleScript.s_singleton.IsDay())
            {
                DeactivateLights(m_LightType.Breaklight);
            }           
        }
        //Toggle the reverselights on if the car is reversing
        if (carscript.CurrentlyReversing == true)
        {
            ActivateLights(m_LightType.Reverselight, 3);
        }
        else
        {
            //Only deactivate the lights if they aren't being toggled by a different event
            if (m_insideTunnel == false || DayNightCycleScript.s_singleton.IsDay())
            {
                DeactivateLights(m_LightType.Reverselight);
            }           
        }
        //Check if inside tunnel


        if (Physics.Raycast(gameObject.transform.position, Vector3.up, 100, LayerMask.GetMask("Default")))
        {
            ActivateLights(m_LightType.Headlight, 8);

            //Only toggle the lights to dim mode if they aren't being handled by the breaking or reversing events

            if (carscript.CurrentlyReversing == false)
            {
                    ActivateLights(m_LightType.Reverselight, 1);
            }
            if (carscript.CurrentlyBraking == false)

            {
                ActivateLights(m_LightType.Breaklight, 1);
            }
            if (carscript.UpsideDown == false)
            {
                ActivateLights(m_LightType.Hazardlight, 1);
            }
            m_insideTunnel = true;
        }

        
        else
        {
            m_insideTunnel = false;
        }
        //If night time
        if (!DayNightCycleScript.s_singleton.IsDay())
		{
            if (m_insideTunnel == false)
            {
                ActivateLights(m_LightType.Headlight, 8);
                //Only toggle the lights to dim mode if they aren't being handled by the breaking or reversing events
                if (carscript.CurrentlyReversing == false)
                {                   
                    ActivateLights(m_LightType.Reverselight, 1);                  
                }
                if (carscript.CurrentlyBraking == false)
                {
                    ActivateLights(m_LightType.Breaklight, 1);
                }
                if (carscript.UpsideDown == false)
                {
                    ActivateLights(m_LightType.Hazardlight, 1);
                }
            }        
		}
        //If daytime and not in a tunnel then the headlights aren't required to be on
        if (DayNightCycleScript.s_singleton.IsDay() && m_insideTunnel == false)
        {
            DeactivateLights(m_LightType.Headlight);
        }

	}

    public void ActivateLights(m_LightType lightType, int intensity)
    {
        
        switch(lightType)
        {
            case m_LightType.Breaklight:
                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_backlights.materials;
                    alternativeMaterials[0] = m_breaklightsON;
                    m_backlights.materials = alternativeMaterials;

                    m_breaklight1.intensity = intensity;
                    m_breaklight2.intensity = intensity;
                }
                if (m_myCarType == m_CarType.JXJ220 || m_myCarType == m_CarType.IceCreamVan || m_myCarType == m_CarType.RV)
                {
                    intensity = 1;
                }
                else
                {
                    m_breaklight1.intensity = intensity;
                    m_breaklight2.intensity = intensity;
                }

                m_breaklight1.intensity = intensity;
                m_breaklight2.intensity = intensity;

                if (m_breaklights)
                {
                    m_breaklights.material = m_breaklightsON;

                }
               
                break;

            case m_LightType.Headlight:
                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_frontlights.materials;
                    alternativeMaterials[0] = m_headlightsON;
                    m_frontlights.materials = alternativeMaterials;
                    m_headlight1.intensity = intensity;
                    m_headlight2.intensity = intensity;
                }
                else
                {
                    m_headlight1.intensity = intensity;
                    m_headlight2.intensity = intensity;
                    if (m_myCarType == m_CarType.Manta || m_myCarType == m_CarType.MGMetro)
                    {
                        m_headlight3.intensity = intensity;
                    }

                    if (m_myCarType == m_CarType.IceCreamVan)
                    {
                        Material[] alternativeMaterials = m_headlights.materials;
                        alternativeMaterials[1] = m_headlightsON;
                        m_headlights.materials = alternativeMaterials;
                    }
                    else
                    {
                        if (m_headlights)
                        {
                            m_headlights.material = m_headlightsON;
                        }
                    }
                    if (m_myCarType == m_CarType.Manta)
                    {
                        Material[] alternativeMaterials = m_foglights.materials;
                        alternativeMaterials[2] = m_headlightsON;
                        m_foglights.materials = alternativeMaterials;

                    }
                    if (m_myCarType == m_CarType.JXJ220)
                    {
                        m_taillights.material = m_taillightsON;
                    }
                }
                

                break;

            case m_LightType.Reverselight:
                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_backlights.materials;
                    alternativeMaterials[1] = m_reverselightsON;
                    m_backlights.materials = alternativeMaterials;
                    m_reverselight1.intensity = intensity;
                    m_reverselight2.intensity = intensity;
                }
                else
                {
                    if (m_myCarType == m_CarType.OpelGT)
                    {
                        m_reverselight1.intensity = intensity;
                        intensity = 0;
                    }
                    if (m_myCarType == m_CarType.RV)
                    {
                        intensity = 0;
                    }
                    if (m_myCarType != m_CarType.IceCreamVan && m_myCarType != m_CarType.OpelGT && m_myCarType != m_CarType.RV)
                    {
                        m_reverselight1.intensity = intensity;
                        m_reverselight2.intensity = intensity;

                        if (m_reverselights)
                        {
                            m_reverselights.material = m_reverselightsON;
                        }
                    }
                    else
                    {
                        if (m_breaklights)
                        {
                            m_breaklights.material = m_breaklightsON;

                        }
                    }
                }
                
                                
                break;

            case m_LightType.Hazardlight:
                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_backlights.materials;
                    alternativeMaterials[2] = m_hazardsON;
                    m_backlights.materials = alternativeMaterials;
                    Material[] alternativeMaterials1 = m_frontlights.materials;
                    alternativeMaterials1[1] = m_hazardsON;
                    m_frontlights.materials = alternativeMaterials1;
                    m_hazardlight1.intensity = intensity;
                    m_hazardlight2.intensity = intensity;
                }
                else
                {
                    m_hazardlight1.intensity = intensity;
                    m_hazardlight2.intensity = intensity;

                    if (m_hazardlightsleft)
                    {
                        m_hazardlightsleft.material = m_hazardsON;
                    }
                    if (m_hazardlightsright)
                    {
                        m_hazardlightsright.material = m_hazardsON;
                    }
                }

                break; 
        }     
    }

    public void DeactivateLights(m_LightType lightType)
    {
        switch (lightType)
        {
            case m_LightType.Breaklight:
                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_backlights.materials;
                    alternativeMaterials[0] = m_breaklightsOFF;
                    m_backlights.materials = alternativeMaterials;
                    m_breaklight1.intensity = 0;
                    m_breaklight2.intensity = 0;
                }
                else
                {
                    m_breaklight1.intensity = 0;
                    m_breaklight2.intensity = 0;

                    if (m_breaklights)
                    {
                        m_breaklights.material = m_breaklightsOFF;
                    }
                }
                
                break;

            case m_LightType.Headlight:

                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_frontlights.materials;
                    alternativeMaterials[0] = m_headlightsOFF;
                    m_frontlights.materials = alternativeMaterials;
                    m_headlight1.intensity = 0;
                    m_headlight2.intensity = 0;
                }
                else
                {
                    m_headlight1.intensity = 0;
                    m_headlight2.intensity = 0;
                    if (m_myCarType == m_CarType.Manta || m_myCarType == m_CarType.MGMetro)
                    {
                        m_headlight3.intensity = 0;
                    }

                    if (m_myCarType == m_CarType.IceCreamVan)
                    {
                        Material[] alternativeMaterials = m_headlights.materials;
                        alternativeMaterials[1] = m_headlightsOFF;
                        m_headlights.materials = alternativeMaterials;
                    }
                    else
                    {
                        if (m_headlights)
                        {
                            m_headlights.material = m_headlightsOFF;
                        }
                    }

                    if (m_myCarType == m_CarType.Manta)
                    {
                        Material[] alternativeMaterials = m_foglights.materials;
                        alternativeMaterials[2] = m_headlightsOFF;
                        m_foglights.materials = alternativeMaterials;
                    }

                    if (m_myCarType == m_CarType.JXJ220)
                    {
                        m_taillights.material = m_taillightsOFF;
                    }
                }

                
                break;

            case m_LightType.Reverselight:

                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_backlights.materials;
                    alternativeMaterials[1] = m_reverselightsOFF;
                    m_backlights.materials = alternativeMaterials;
                    m_reverselight1.intensity = 0;
                    m_reverselight2.intensity = 0;
                }
                if (m_myCarType == m_CarType.OpelGT)
                {
                    m_reverselight1.intensity = 0;
                }
                else
                {
                    if (m_myCarType != m_CarType.IceCreamVan && m_myCarType != m_CarType.OpelGT && m_myCarType != m_CarType.RV)
                    {
                        m_reverselight1.intensity = 0;
                        m_reverselight2.intensity = 0;

                        if (m_reverselights)
                        {
                            m_reverselights.material = m_reverselightsOFF;
                        }
                    }
                    else
                    {
                        if (m_breaklights)
                        {
                            if (carscript.CurrentlyBraking == false)
                            {
                                m_breaklights.material = m_breaklightsOFF;
                            }
                        }
                    }
                }              

                break;

            case m_LightType.Hazardlight:
                if (m_myCarType == m_CarType.LotusCarlton)
                {
                    Material[] alternativeMaterials = m_backlights.materials;
                    alternativeMaterials[2] = m_hazardsOFF;            
                    m_backlights.materials = alternativeMaterials;
                    Material[] alternativeMaterials1 = m_frontlights.materials;
                    alternativeMaterials1[1] = m_hazardsOFF;
                    m_frontlights.materials = alternativeMaterials1;

                    m_hazardlight1.intensity = 0;
                    m_hazardlight2.intensity = 0;
                }
                else
                {
                    m_hazardlight1.intensity = 0;
                    m_hazardlight2.intensity = 0;

                    if (m_hazardlightsleft)
                    {
                        m_hazardlightsleft.material = m_hazardsOFF;
                    }
                    if (m_hazardlightsright)
                    {
                        m_hazardlightsright.material = m_hazardsOFF;
                    }
                }
                
                break;
        }
        
    }

}
