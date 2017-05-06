using UnityEngine;
using System.Collections.Generic;

public class AddonManager : MonoBehaviour {

	public enum AddonType_e
	{
		NONE = 0,
		CARAVAN = 1,
		GRAPPLE = 2,
		GLIDER = 3
	}

    public static AddonManager m_instance = null;

    [SerializeField] Transform grappleLauncher;
	[SerializeField] Transform caravan;
	[SerializeField] Transform glider;

	List<Transform> cars = new List<Transform> ();

    void Start()
    {
        if (m_instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }
        setupPlayers();

    }

    private void setupPlayers()
    {
        cars.Clear();
        int playerCount = Kojima.GameController.s_singleton.m_players.Length;
        for (int i = 0; i < playerCount; i++)
        {
            if (Kojima.GameController.s_singleton.m_players[i] != null)
            {
                cars.Add(Kojima.GameController.s_singleton.m_players[i].transform);
            }
        }
    }

    public List<Transform> getAllCars()
    {
        return cars;
    }

    public void addToAllCars(AddonType_e _Type)
    {
        setupPlayers();
        foreach (Transform car in cars)
        {
            addObjectToCar(_Type, car);
        }
    }

    public void PickupConverter(int _AddonRef, int _CarRef, bool _Add)
    {
        setupPlayers();
        AddonType_e n_Type = AddonType_e.NONE;
        switch (_AddonRef)
        {
            case 1:
                n_Type = AddonType_e.CARAVAN;
                break;
            case 2:
                n_Type = AddonType_e.GRAPPLE;
                break;
            case 3:
                n_Type = AddonType_e.GLIDER;
                break;
        }
        if (_Add)
        {
            addObjectToCar(n_Type, cars[_CarRef]);
        }
        else
        {
            destroyAddon(n_Type, cars[_CarRef]);
        }
    }

	public void addObjectToCar(AddonType_e _Type, Transform _Car)
	{
        switch (_Type)
		{
		case AddonType_e.CARAVAN:
			addCaravan (_Car);
			break;
		case AddonType_e.GRAPPLE:
			addGrapple (_Car);
			break;
		case AddonType_e.GLIDER:
			addGlider (_Car);
			break;
		default:
			break;
		}
	}

	void addCaravan(Transform _Car)
	{
		if(_Car != null)
		{
			if(!_Car.GetComponent<CaravanManager>())
			{
				_Car.gameObject.AddComponent<CaravanManager>();
			}
			_Car.GetComponent<CaravanManager> ().addCaravan (caravan);
		}
	}

	void addGrapple(Transform _Car)
	{
		if(_Car != null && !_Car.GetComponent<GrappleLaunchManager>())
		{
			Vector3 spawnPos = _Car.GetComponent<Bam.CarSockets> ().GetSocket (Bam.CarSockets.Sockets.LowFront).position;
			Transform newGrappleLauncher = Instantiate (grappleLauncher, spawnPos, _Car.rotation) as Transform;
			newGrappleLauncher.parent = _Car;
			_Car.gameObject.AddComponent<GrappleLaunchManager>();
            _Car.GetComponent<GrappleLaunchManager>().setGrappleLauncher(newGrappleLauncher);
            newGrappleLauncher.GetComponent<GrappleLaunch>().init();

        }
	}

	void addGlider(Transform _Car)
	{
		if(_Car != null)
		{
            if (!_Car.GetComponentInChildren<Glider>())
            {
                _Car.GetComponent<Kojima.CarScript>().PullOutGlider();
                Vector3 spawnPos = _Car.GetComponent<Bam.CarSockets>().GetSocket(Bam.CarSockets.Sockets.Bonnet).position;
                Transform newGlider = Instantiate(glider, spawnPos, _Car.transform.rotation * glider.rotation) as Transform;
                newGlider.parent = _Car;
                newGlider.GetComponent<ToggleGlider>().m_Open = true;
            } 
		}
	}

	public void destroyAllAddons()
	{
        setupPlayers();

        foreach (Transform car in cars)
		{
			destroyAddon (AddonType_e.CARAVAN, car);
			destroyAddon (AddonType_e.GLIDER, car);
			destroyAddon (AddonType_e.GRAPPLE, car);
		}
	}

    public void destroyAddon(AddonType_e _Type, Transform _Car)
    {
        if (_Car != null)
        {
            switch (_Type)
            {
			case AddonType_e.CARAVAN:
				if (_Car.GetComponent<CaravanManager>()) {
					foreach (Transform caravan in _Car.GetComponent<CaravanManager>().m_Caravans) {
						caravan.GetComponent<CaravanController> ().detach ();
					}
				}
                    break;
                case AddonType_e.GRAPPLE:
                    if (_Car.GetComponentInChildren<GrappleLaunchManager>())
                    {
                        Destroy(_Car.GetComponent<GrappleLaunchManager>().getGrappleLauncher().gameObject);
                        Destroy(_Car.GetComponent<GrappleLaunchManager>());
                    }
                    break;
                case AddonType_e.GLIDER:
                    if (_Car.GetComponentInChildren<Glider>())
                    {
                        _Car.GetComponent<Kojima.CarScript>().PutAwayGlider();
                        //Destroy(_Car.GetComponentInChildren<Glider>().gameObject);
                        GameObject _Glider = _Car.GetComponentInChildren<Glider>().gameObject;
                        _Glider.transform.parent = null;
                        _Glider.AddComponent<Rigidbody>();
                        _Glider.AddComponent<Draggable>();

                    }
                    break;
                default:
                    break;
            }
        }
    }
}