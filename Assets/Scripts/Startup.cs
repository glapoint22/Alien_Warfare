using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Startup : MonoBehaviour
{
    [SerializeField]
    private GameObject spaceBackgroundPrefab;

    [SerializeField]
    private GameObject localizationPrefab;

    [SerializeField]
    private Version version;
    private enum Version { Development, Live}

    IEnumerator Start()
    {
        if(version == Version.Live)
        {
            //Create the assetBundles object
            AssetBundles assetBundles = new AssetBundles();

            //Get the asset bundle versions
            yield return assetBundles.GetAssetBundleVersions();

            //Get the manifest
            yield return assetBundles.GetManifest();

            //Set the variants
            string systemLanguage = Application.systemLanguage.ToString().ToLower();
            assetBundles.variants.Add(systemLanguage);

            //Load the space background
            yield return assetBundles.LoadGameObjectFromAssetBundle("space_background", "Space Background");
            DontDestroyOnLoad(assetBundles.asset);

            //Download the asset bundles
            yield return assetBundles.DownloadAssetBundles();

            //Load the localization gameobject
            yield return assetBundles.LoadGameObjectFromAssetBundle("localization", "Localization");
            DontDestroyOnLoad(assetBundles.asset);

            //Load the login scene
            yield return assetBundles.LoadScene("scenes/login", "Login", false);
        }
        else
        {
            //Instantiate the space background
            GameObject spaceBackground = Instantiate(spaceBackgroundPrefab);
            DontDestroyOnLoad(spaceBackground);

            //Instantiate the localization
            GameObject localization = Instantiate(localizationPrefab);
            DontDestroyOnLoad(localization);

            //Load the login scene
            SceneManager.LoadScene(1);
        }
    }
}


