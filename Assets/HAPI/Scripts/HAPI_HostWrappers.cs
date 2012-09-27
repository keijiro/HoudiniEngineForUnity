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
 * 		Continuation of HAPI_Host class definition. Here we include all public wrappers for the dll imports defined
 * 		in HAPI_HostImports.cs.
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace HAPI 
{	
	/// <summary>
	/// 	Singleton Houdini host object that maintains the singleton Houdini scene and all access to the
	/// 	Houdini runtime.
	/// </summary>
	public static partial class HAPI_Host
	{		
		// GENERICS -------------------------------------------------------------------------------------------------
		
		/// <summary>	
		/// 	Converts a transform to differing TRS order and Euler rotation order	    
		/// </summary>
	    ///
	    /// <param name= "transform_in_out">	    
	    ///			Used for both input and output.
		/// </param>
	    /// <param name="rst_order">
	    ///         input. The desired transform order of the output
	    /// 		TRS = 0, TSR = 1, RTS = 2, RST = 3, STR = 4, SRT = 5    
	    /// </param>	    
	    ///	<param name="rot_order">
	    ///			The desired rotation order of the output        
	    ///         XYZ = 0, XZY = 1, YXZ = 2, YZX = 3, ZXY = 4, ZYX = 5
	    /// </param>
	    public static void convertTransform( 	ref HAPI_TransformEuler transform_in_out, 
	                                     		int rst_order, int rot_order )
		{
			int status_code = HAPI_ConvertTransform( ref transform_in_out, rst_order, rot_order );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		
		/// <summary>	
		/// 	Converts a 4x4 matrix into its TRS form.  
		/// </summary>
	    ///		
	    /// <param name="mat">
	    ///                 A 4x4 matrix expressed in a 16 element float array
	    /// </param>
	    /// <param name="rst_order">
	    ///         The desired transform order of the output
	    ///			TRS = 0, TSR = 1, RTS = 2, RST = 3, STR = 4, SRT = 5    
		/// </param>
	    ///	<param name="rot_order">
	    ///			The desired rotation order of the output        
	    ///         XYZ = 0, XZY = 1, YXZ = 2, YZX = 3, ZXY = 4, ZYX = 5
	    /// </param>
	    /// <param name="transform_out">
	    ///			Used for the output.
	    /// </param>
		public static void convertMatrix( 	    float[] mat,
	                                     		int rst_order, int rot_order,
												ref HAPI_TransformEuler transform_out )
		{
			int status_code = HAPI_ConvertMatrix( mat, rst_order, rot_order, ref transform_out );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		
		/// <summary>
		/// 	An utility function that computes a transform based on parameters
		/// 	typical to instancing.  
		/// </summary>	    
	    /// <param name="transform_inst">
	    ///                 A HAPI_TransformInstance structure describing parameters
	    ///                 relevant to instancing.  See HAPI_TransformInstance for 
	    ///                 details
	    /// </param>
	    /// <param name="rst_order">
	    ///                 The desired transform order of the output
	    ///			TRS = 0, TSR = 1, RTS = 2, RST = 3, STR = 4, SRT = 5        
		/// </param>
	    /// <param name="transform_out">
	    ///			Used for the output.
		///	</param>
		public static void computeInstanceTransform( ref HAPI_TransformInstance transform_inst,
                                             		int rst_order,
                                             		ref HAPI_Transform transform_out )
		{
			int status_code = HAPI_ComputeInstanceTransform( ref transform_inst,
															 rst_order,
															 ref transform_out);
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		// STRINGS --------------------------------------------------------------------------------------------------
		
		// NOTE: These are private as we want people to use the more abstract methods in HAPI_HostStrings.cs.
		
		/// <summary>
		/// 	Gives back the string length of the string with the given handle.
		/// </summary>
		/// <param name="string_handle">
		/// 	Handle of the string to query.
		/// </param>
		/// <param name="string_length">
		/// 	Length of the queried string.
		/// </param>
		private static void getStringLength( 	int string_handle, 
												out int string_length )
		{
			int status_code = HAPI_GetStringLength( string_handle, out string_length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Gives back the string value of the string with the given handle.
		/// </summary>
		/// <param name="string_handle">
		/// 	Handle of the string to query.
		/// </param>
		/// <param name="string_value">
		/// 	Actual string value.
		/// </param>
		/// <param name="string_length">
		/// 	Length of the queried string (must match size of <paramref name="string_value">).
		/// </param>
		private static void getString(		 	int string_handle,
												StringBuilder string_value,
												int string_length )
		{
			int status_code = HAPI_GetString( string_handle, string_value, string_length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Sets the string value of the string with the given handle.
		/// </summary>
		/// <param name="string_handle">
		/// 	Handle of the string to set.
		/// </param>
		/// <param name="string_value">
		/// 	Actual string value.
		/// </param>
		/// <param name="string_length">
		/// 	Length of the queried string (must match size of <paramref name="string_value">).
		/// </param>
		private static void setString( 			int string_handle,
												string string_value,
												int string_length )
		{
			int status_code = HAPI_SetString( string_handle, string_value, string_length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		// PARAMETERS -----------------------------------------------------------------------------------------------
		
		/// <summary>
		/// 	Fill an array of HAPI_ParmInfo structs with parameter information from the asset instance node.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="parm_infos">
		/// 	Array of <see cref="HAPI_ParmInfo"/> at least the size of 
		/// 	<paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AssetInfo.parmCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AssetInfo.parmCount"/> - <paramref name="start"/>.
		/// </param>
		public static void getParameters( 	int asset_id, 
											[Out] HAPI_ParmInfo[] parm_infos, 
											int start, int end )
		{
			int status_code = HAPI_GetParameters( asset_id, parm_infos, start, end );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		/// <summary>
		/// 	Fill an array of <see cref="HAPI_ParmSingleValue"/> structs with extra parameter vector fields.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="parm_extra_values">
		/// 	Array of <see cref="HAPI_ParmSingleValue"/> exactly the size of 
		/// 	<see cref="HAPI_AssetInfo.parmExtraValueCount"/>.
		/// </param>
		/// <param name="count">
		/// 	Sanity check. Must be equal to <see cref="HAPI_AssetInfo.parmExtraValueCount"/>.
		/// </param>
		public static void getParmExtraValues( 	int asset_id, 
												[Out] HAPI_ParmSingleValue[] parm_extra_values, 
												int start, int length )
		{
			int status_code = HAPI_GetParmExtraValues( asset_id, parm_extra_values, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		/// <summary>
		/// 	Fill an array of <see cref="HAPI_ParmChoiceInfo"/> structs with parameter choice list information 
		/// 	from the asset instance node.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="parm_choices">
		/// 	Array of <see cref="HAPI_ParmChoiceInfo"/> exactly the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AssetInfo.parmChoiceCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AssetInfo.parmChoiceCount"/> - <paramref name="start"/>.
		/// </param>
		public static void getParmChoiceLists( 	int asset_id, 
												[Out] HAPI_ParmChoiceInfo[] parm_choices, 
												int start, int length )
		{
			int status_code = HAPI_GetParmChoiceLists( asset_id, parm_choices, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		/// <summary>
		/// 	Set a subset of parameter values using the given array of <see cref="HAPI_ParmInfo"/>.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="parm_infos">
		/// 	Array of <see cref="HAPI_ParmInfo"/> at least the size of 
		/// 	<paramref name="length"/> containing the new parameter values.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AssetInfo.parmCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AssetInfo.parmCount"/> - <paramref name="start"/>.
		/// </param>
		public static void setParameters( 		int asset_id, 
												[Out] HAPI_ParmInfo[] parm_infos, 
												int start, int length )
		{
			int status_code = HAPI_SetParameters( asset_id, parm_infos, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Set an array of <see cref="HAPI_ParmSingleValue"/> structs with extra parameter vector fields.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="parm_extra_values">
		/// 	Array of <see cref="HAPI_ParmSingleValue"/> exactly the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most 
		/// 	<see cref="HAPI_AssetInfo.parmExtraValueCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most 
		/// 	<see cref="HAPI_AssetInfo.parmExtraValueCount"/> - <paramref name="start"/>.
		/// </param>
		public static void setParmExtraValues( 	int asset_id, 
												[Out] HAPI_ParmSingleValue[] parm_extra_values, 
												int start, int length )
		{
			int status_code = HAPI_SetParmExtraValues( asset_id, parm_extra_values, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		// HANDLES --------------------------------------------------------------------------------------------------
		
		/// <summary>	
		/// 	Fill an array of HAPI_HandleInfo structs with information
	    ///		about every exposed user manipulation handle on the asset    
		/// </summary>
	    ///
	    /// <param name="asset_id">
	    ///		The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
	    /// <param name ="handle_infos">
	    ///		Array of <see cref="HAPI_HandleInfo"/> exactly the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AssetInfo.handleCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AssetInfo.handleCount"/> - <paramref name="start"/>.
		/// </param>
	    public static void getHandleInfo(	int asset_id, 
											[Out] HAPI_HandleInfo[] handle_infos,
											int start, int length )
		{
			int status_code = HAPI_GetHandleInfo( asset_id, handle_infos, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );				
		}
	
		
		/// <summary>	
		/// 	Fill an array of HAPI_HandleBindingInfo structs with information
	    ///		about how each handle parameter maps to each asset parameter
		/// </summary>
	    ///
	    /// <param name="asset_id">
	    ///		The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="handle index">
	    ///		The index of the handle, from 0 to handleCount - 1 from the call to <see cref="HAPI_Host.loadOTLFile"/>
		/// </param>		
		/// <param name ="handle_infos">
	    ///		Array of <see cref="HAPI_HandleBindingInfo"/> exactly the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_HandleInfo.bindingsCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_HandleInfo.bindingsCount"/> - <paramref name="start"/>.
		/// </param>
	    public static void getHandleBindingInfo(	int asset_id,
	                                         		int handle_index,
													[Out] HAPI_HandleBindingInfo[] handle_infos,
													int start, int length )
		{
			int status_code = HAPI_GetHandleBindingInfo( asset_id, handle_index, handle_infos, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		// OBJECTS --------------------------------------------------------------------------------------------------
		
		/// <summary>
		/// 	Fill an array of <see cref="HAPI_ObjectInfo"/> structs with information on each visible object 
		/// 	in the scene that has a SOP network (is not a sub-network).
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_infos">
		/// 	Array of <see cref="HAPI_ObjectInfo"/> at least the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AssetInfo.objectCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AssetInfo.objectCount"/> - <paramref name="start"/>.
		/// </param>
		public static void getObjects( 			int asset_id, 
												[Out] HAPI_ObjectInfo[] object_infos, 
												int start, int length )
		{
			int status_code = HAPI_GetObjects( asset_id, object_infos, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		/// <summary>
		/// 	Fill an array of <see cref="HAPI_ObjectInfo"/> structs with information on each visible object 
		/// 	in the scene that has a SOP network (is not a sub-network).
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="rst_order">
	    ///		The order of application of translation, rotation and
	    ///     scale:
	    ///		TRS = 0, TSR = 1, RTS = 2, RST = 3, STR = 4, SRT = 5	    
	    /// </param>
		/// <param name="transforms">
		/// 	Array of <see cref="HAPI_Transform"/> at least the size of 
		/// 	<paramref name="length"/>. The <see cref="HAPI_Transform.id"/> of each will be 
    	/// 	set to the object id as given by <see cref="HAPI_Host.HAPI_GetObjects"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AssetInfo.objectCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AssetInfo.objectCount"/> - <paramref name="start"/>.
		/// </param>
		public static void getObjectTransforms(	int asset_id, 
												int rst_order,
												[Out] HAPI_Transform[] transforms,
												int start, int length )
		{
			int status_code = HAPI_GetObjectTransforms( asset_id, rst_order, transforms, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );	
		}
		
		// DETAILS --------------------------------------------------------------------------------------------------
		
		/// <summary>
		/// 	Get the main detail/geometry info struct (<see cref="HAPI_DetailInfo"/>).
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="detail_info">
		/// 	<see cref="HAPI_DetailInfo"/> out parameter.
		/// </param>
		public static void getDetailInfo(		int asset_id, int object_id,
												out HAPI_DetailInfo detail_info )
		{
			int status_code = HAPI_GetDetailInfo( asset_id, object_id, out detail_info );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get the array of faces where the nth integer in the array is the number of vertices
		/// 	the nth face has.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="face_counts">
		/// 	An integer array at least the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_DetailInfo.faceCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_DetailInfo.faceCount"/> - <paramref name="start"/>.
		/// </param>
		public static void getFaceCounts(		int asset_id, int object_id,
												[Out] int[] face_counts,
												int start, int length )
		{
			int status_code = HAPI_GetFaceCounts( asset_id, object_id, face_counts, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get array containing the vertex-point associations where the ith element in the array is
		/// 	the point index the ith vertex associates with.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="vertex_list">
		/// 	An integer array at least the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_DetailInfo.vertexCount"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_DetailInfo.vertexCount"/> - <paramref name="start"/>.
		/// </param>
		public static void getVertexList(		int asset_id, int object_id,
												[Out] int[] vertex_list,
												int start, int length )
		{
			int status_code = HAPI_GetVertexList( asset_id, object_id, vertex_list, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get attribute information; fill a <see cref="HAPI_AttributeInfo"/>.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="attr_info">
		/// 	<see cref="HAPI_AttributeInfo"/> used as input for which (by name) attribute you want the info 
		/// 	for and the owner type and as output for the rest of the information.
		/// </param>
		public static void getAttributeInfo(	int asset_id, int object_id,
												ref HAPI_AttributeInfo attr_info )
		{
			int status_code = HAPI_GetAttributeInfo( asset_id, object_id, ref attr_info );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get list of attribute names by attribute owner.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="attribute_type"/>
		/// 	The <see cref="HAPI_AttributeType"/> enum value specifying the owner of the attribute.
		/// </param>
		/// <param name="data">
		/// 	Array of strings (<see cref="HAPI_AttributeStrValue"/>) to house the attribute names.
		/// 	Should be exactly the size of the appropriate attribute owner type count 
		/// 	in <see cref="HAPI_DetailInfo"/>.
		/// </param>
		/// <param name="count">
		/// 	Sanity check count. Must be equal to the appropriate attribute owner type count 
		/// 	in <see cref="HAPI_DetailInfo"/>.
		/// </param>
		public static void getAttributeNames(	int asset_id, int object_id,
												int attribute_type,
												[Out] HAPI_AttributeStrValue[] data,
												int count )
		{
			int status_code = HAPI_GetAttributeNames( asset_id, object_id, attribute_type, data, count );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get attribute integer data.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="attr_info">
		/// 	<see cref="HAPI_AttributeInfo"/> used as input for which attribute you want the data for and
		/// 	in what tuple size. Also contains some sanity checks like data type. Generally should be 
		/// 	the same struct returned by <see cref="HAPI_Host.getAttributeInfo"/>.
		/// </param>
		/// <param name="data">
		/// 	An integer array at least the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AttributeInfo.count"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AttributeInfo.count"/> - <paramref name="start"/>.
		/// </param>
		public static void getAttributeIntData( int asset_id, int object_id,
												ref HAPI_AttributeInfo attr_info,
												[Out] int[] data,
												int start, int length )
		{
			int status_code = HAPI_GetAttributeIntData( asset_id, object_id, ref attr_info, data, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get attribute float data.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="attr_info">
		/// 	<see cref="HAPI_AttributeInfo"/> used as input for which attribute you want the data for and
		/// 	in what tuple size. Also contains some sanity checks like data type. Generally should be 
		/// 	the same struct returned by <see cref="HAPI_Host.getAttributeInfo"/>.
		/// </param>
		/// <param name="data">
		/// 	An float array at least the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AttributeInfo.count"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AttributeInfo.count"/> - <paramref name="start"/>.
		/// </param>
		public static void getAttributeFloatData(	int asset_id, int object_id,
													ref HAPI_AttributeInfo attr_info,
													[Out] float[] data,
													int start, int length )
		{
			int status_code = HAPI_GetAttributeFloatData( asset_id, object_id, ref attr_info, data, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}
		
		/// <summary>
		/// 	Get attribute string data.
		/// </summary>
		/// <param name="asset_id">
		/// 	The asset id returned by <see cref="HAPI_Host.loadOTLFile"/>.
		/// </param>
		/// <param name="object_id">
		/// 	The object id returned by <see cref="HAPI_Host.getObjects"/>.
		/// </param>
		/// <param name="attr_info">
		/// 	<see cref="HAPI_AttributeInfo"/> used as input for which attribute you want the data for and
		/// 	in what tuple size. Also contains some sanity checks like data type. Generally should be 
		/// 	the same struct returned by <see cref="HAPI_Host.getAttributeInfo"/>.
		/// </param>
		/// <param name="data">
		/// 	An string (<see cref="HAPI_AttributeStrValue"/>) array at least the size of <paramref name="length"/>.
		/// </param>
		/// <param name="start">
		/// 	First index of range. Must be at least 0 and at most <see cref="HAPI_AttributeInfo.count"/> - 1.
		/// </param>
		/// <param name="length">
		/// 	Must be at least 0 and at most <see cref="HAPI_AttributeInfo.count"/> - <paramref name="start"/>.
		/// </param>
		public static void getAttributeStrData( int asset_id, int object_id,
												ref HAPI_AttributeInfo attr_info,
												[Out] HAPI_AttributeStrValue[] data,
												int start, int length )
		{
			int status_code = HAPI_GetAttributeStrData( asset_id, object_id, ref attr_info, data, start, length );
			processStatusCode( (HAPI_StatusCode) status_code );
		}	
	}

}