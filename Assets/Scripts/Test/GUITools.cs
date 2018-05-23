using System.Text;
using UnityEngine;

namespace TriPeaks
{
	public class GUITools : MonoBehaviour 
	{
//		public Transform Trans { get; private set; }
		
//		public GameObject GameObj { get; private set; }
		
		void Awake () 
		{
//			Trans = transform;
//			GameObj = gameObject;
			
			
		}
		
		void OnGUI ()
		{
		    GUI.skin.button.fontSize = 64;
		    if (GUILayout.Button("Length"))
		    {
		        StringBuilder sb = new StringBuilder();
		        sb.Append(@"SELECT count(*),`Column1`,`Testing`, `Testing Three` FROM `Table1`
    WHERE Column1 = 'testing' AND ( (`Column2` = `Column3` OR Column4 >= NOW()) )
    GROUP BY Column1 ORDER BY Column3 DESC LIMIT 5,10");
                Debug.LogError(sb.Length);
		    }
		}
	}
}