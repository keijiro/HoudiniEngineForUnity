/*
 * PROPRIETARY INFORMATION.  This software is proprietary to
 * Side Effects Software Inc., and is not to be reproduced,
 * transmitted, or disclosed in any way without written permission.
 *
 * Produced by:
 *      Side Effects Software Inc
 *		123 Front Street West, Suite 1401
 *		Toronto, Ontario
 *		Canada   M5J 2M2
 *		416-504-9876
 *
 * COMMENTS:
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using HAPI;

// Typedefs
using HAPI_NodeId = System.Int32;

public class HAPI_GeoControl : HAPI_ObjectControl 
{

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Public Properties
	
	// Please keep these in the same order and grouping as their initializations in HAPI_Control.reset().

	public int prGeoId {				get { return myGeoId; }		set { myGeoId = value; } }
	public string prGeoName {			get { return myGeoName; }	set { myGeoName = value; } }
	public HAPI_GeoType prGeoType {		get { return myGeoType; }	set { myGeoType = value; } }
	public HAPI_ObjectControl prObjectControl { get { return myObjectControl; } set { myObjectControl = value; } }

	public List< GameObject > prParts {	get { return myParts; }		set { myParts = value; } }

	public HAPI_GeoControl() 
	{
		reset();
	}

	~HAPI_GeoControl()
	{

	}

	public override void reset()
	{
		base.reset();

		// Please keep these in the same order and grouping as their declarations at the top.
		
		prGeoId			= -1;
		prGeoName		= "geo_name";
		prGeoType		= HAPI_GeoType.HAPI_GEOTYPE_DISPLAY;
		prObjectControl	= null;

		myParts			= new List< GameObject >( 0 );
	}

	public void init( HAPI_GeoControl geo_control )
	{
		init( (HAPI_ObjectControl) geo_control );

		prGeoId			= geo_control.prGeoId;
		prGeoName		= geo_control.prGeoName;
		prGeoType		= geo_control.prGeoType;
	}

	public void init( HAPI_NodeId node_id, int geo_id, string geo_name, HAPI_GeoType geo_type )
	{
		prNodeId		= node_id;
		prGeoId			= geo_id;
		prGeoName		= geo_name;
		prGeoType		= geo_type;
	}

	public void refresh( bool reload_asset )
	{
		if ( prObjectControl == null )
		{
			Debug.LogError( "Why is my object control null on a refresh?" );
			return;
		}

		GameObject geo_node = gameObject;

		// Get Geo info.
		HAPI_GeoInfo geo_info = new HAPI_GeoInfo();
		HAPI_Host.getGeoInfo( prAssetId, prObjectId, prGeoId, out geo_info );

		if ( geo_info.type == HAPI.HAPI_GeoType.HAPI_GEOTYPE_INPUT )
			return;

		if ( !reload_asset && !geo_info.hasGeoChanged && !geo_info.hasMaterialChanged )
			return;

		if ( reload_asset || geo_info.type == HAPI.HAPI_GeoType.HAPI_GEOTYPE_CURVE )
		{
			for ( int i = 0; i < myParts.Count; ++i )
				HAPI_AssetUtility.destroyGameObject( myParts[ i ] );
			myParts.Clear();
		}

		if ( reload_asset || geo_info.hasGeoChanged )
		{
			// Initialize our geo control.
			init( geo_info.nodeId, prGeoId, geo_info.name, (HAPI_GeoType) geo_info.type );

			// Set node name.
			geo_node.name = prGeoName + "_geo" + prGeoId;
		}

		if ( geo_info.type == HAPI.HAPI_GeoType.HAPI_GEOTYPE_CURVE )
		{
			createCurve( geo_info.nodeId, prObjectId, prGeoId, geo_info.isEditable );
		}
		else
		{
			if ( reload_asset || geo_info.hasGeoChanged )
			{
				// Add new geos as needed.
				while ( myParts.Count < geo_info.partCount )
					myParts.Add( createPart( myParts.Count ) );

				// Remove stale geos.
				while ( myParts.Count > geo_info.partCount )
				{
					HAPI_AssetUtility.destroyGameObject( myParts[ geo_info.partCount ] );
					myParts.RemoveAt( geo_info.partCount );
				}
			}
		
			// Refresh all geos.
			for ( int i = 0; i < myParts.Count; ++i )
				myParts[ i ].GetComponent< HAPI_PartControl >().refresh( 
					reload_asset, geo_info.hasGeoChanged, geo_info.hasMaterialChanged );

			if ( reload_asset && geo_info.partCount > 0 )
			{
				HAPI_AttributeInfo script_attr_info = new HAPI_AttributeInfo( "Unity_Script" );
				int[] script_attr = new int[ 0 ];
			
				HAPI_AssetUtility.getAttribute( 
					prAssetId, prObjectId, prGeoId, 0, "Unity_Script",
					ref script_attr_info, ref script_attr, HAPI_Host.getAttributeStrData );
			
				if ( script_attr_info.exists && script_attr_info.owner != (int) HAPI_AttributeOwner.HAPI_ATTROWNER_DETAIL )
					throw new HAPI_ErrorIgnorable( "I only understand Unity_Script as detail attributes!" );
			
				if ( script_attr_info.exists && script_attr.Length > 0 )
				{
					string script_to_attach = HAPI_Host.getString( script_attr[ 0 ] );
					HAPI_AssetUtility.attachScript( geo_node, script_to_attach );
				}
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Private Methods

	private void createCurve( int node_id, int object_id, int geo_id, bool editable )
	{
		HAPI_Parms parms = getOrCreateComponent< HAPI_Parms >();
		parms.prControl = this;
		parms.getParameterValues();

		HAPI_Curve curve = getOrCreateComponent< HAPI_Curve >();
		curve.prControl = this;
		curve.prParms = parms;

		try
		{
			curve.syncPointsWithParm();
			curve.createObject( object_id, geo_id );
		}
		catch ( HAPI_Error )
		{
			// Per-object errors are not re-thrown so that the rest of the asset has a chance to load.
		}
	}

	private GameObject createPart( int part_id )
	{
		GameObject child = new GameObject( "uninitialized_part" );
		child.transform.parent = gameObject.transform;

		// Need to reset position here because the assignment above will massage the child's
		// position in order to be in the same place it was in the global namespace.
		child.transform.localPosition	= new Vector3();
		child.transform.localRotation	= new Quaternion();
		child.transform.localScale		= new Vector3( 1.0f, 1.0f, 1.0f );

		HAPI_PartControl control = child.AddComponent< HAPI_PartControl >();
		control.init( this );
		control.prPartId = part_id;
		control.prGeoControl = this;

		return child;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Serialized Private Data

	[SerializeField] private int			myGeoId;
	[SerializeField] private string			myGeoName;
	[SerializeField] private HAPI_GeoType	myGeoType;
	[SerializeField] private HAPI_ObjectControl myObjectControl;

	[SerializeField] private List< GameObject > myParts;
}
