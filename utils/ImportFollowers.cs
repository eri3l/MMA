using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using app.mma;

namespace app.mma.utils{
	public class ImportFollowers{

		public void ImportTest (string[] args)
		{
			string filename;
			string separator = "" + '\t';

			if ( args.Length < 2 ){
				Console.WriteLine ("Enter followers import file and optional delimiter (default is tab)");
				filename = "followers.tsv";
				Console.WriteLine ("<followers.tsv>?:");
				string cms = Console.ReadLine ();
				if (!string.IsNullOrEmpty (cms.Trim()))
					filename = cms;
			}
			else {
				filename = args [1];
				if (args.Length == 2)
					separator = args [2];
			}

			CsvParserLines csvl=null;
			Ent ent=null;
			Doc doc=null;
			Edge edg=null;
			Acc acc=null;
			Geo geo=null;
			Lang lang=null;
			Info info=null;
			string alias = null;
			string criterion = "default";
			object dbConn = null; // TODO: replace with real db connection
			string[] line = {};
			int prop = 0;
			string[] props;

			try {
				csvl = new CsvParserLines( filename, separator );
				ent = new Ent();
				doc = new Doc ();
				edg = new Edge ();
				acc = new Acc ();
				geo = new Geo ();
				lang = new Lang ();
				info = new Info ();
				dbConn = null; // TODO: replace with real db connection 
			} catch (Exception ex) {
				Console.WriteLine ("Couldn't initiallise import of file: {0}\n error = {1}\nstacktrace=:", filename, ex.Message, ex.StackTrace);
				System.Environment.Exit(1);
			}

			props = csvl [0];
			for(int i = 1; i < csvl.Count; i++){
				try { //parse a line

					line = csvl [i];

					#region create bags for each follower/fan entry/import
					// TODO: leave dicts here for debug, but move one above later
					Dictionary<string, object> entNewData = new Dictionary<string, object>();
					Dictionary<string, object> infoNewData = new Dictionary<string, object>();
					Dictionary<string, object> langNewData = new Dictionary<string, object>();
					Dictionary<string, object> edgeNewData = new Dictionary<string, object>();
					Dictionary<string, object> accNewData = new Dictionary<string, object>();
					Dictionary<string, object> geoNewData = new Dictionary<string, object>();
					Dictionary<string, object> docNewData = new Dictionary<string, object>();


					for (prop=0; prop < props.Length; prop++){

						line[prop] = ((string)line[prop]).Trim();

						switch (props[prop]){


						case "statusesCount":
						case "followersCount":
						case "favoritesCount":
						case "friendsCount":
						case "listedCount":
							edgeNewData [props [prop]] = Int32.Parse(line[prop]); 
							break;

						case "description":
							// string : nodedat
							entNewData["nodedat"] = line[prop];
							break;

						case "verified":
							float verif = line[prop] == "TRUE" ?  1.0f: 0.0f;  
							accNewData["nodevld"] = verif;
							entNewData ["nodevld"] = verif/2; 
							break;

						case "url":
							// add new val to Entinfo
							infoNewData["eninval"] = line[prop];
							break;

						case "location":
							// rawaddress
							infoNewData["rawaddress"] = line[prop];
							break;

						case "lang":
							langNewData[props[prop]] = line[prop];
							break;

						case "profileImageUrl":
							docNewData["docudat"] = line[prop];
							break;

						case "name":
							// this is the user-supplied name
							// 1. create a modified Ent.snm in some way
							entNewData["enttsnm"] = "tw." + line[prop];
							// 2. write to field acctsnm in the acct dict
							accNewData["acctsnm"] = line[prop];
							break;

						case "screenName":
							// this is the twitter unique nm
							// 1. create a modified Ent.nm
							entNewData["enttnm"] = "tw." + line[prop] + ".001";
							// 2. write to field acctnm
							accNewData["acctnm"] = line[prop];
							break;

						case "id":
							// write to field acctforeignid
							accNewData["acctforeignid"] = Int64.Parse(line[prop]);
							break;

						case "created":
							DateTime date = DateTime.Parse(line[prop], new CultureInfo("en-US", false));
							accNewData["nodebirthd"] = date; // TODO DateTime convert 
							break;

						case "geolcntry":
						case "geolb":
						case "locality":
						case "area_2":
						case "area_1":
							geoNewData[props[prop]] = line[prop];
							break;

							//default:
							// TODO: error unrecognised column name

						} //switch prop
					} //for props

					#endregion // creat bags

					#region geo
					string sb = (string)geoNewData["geolb"];
					float xll, yll, xur, yur;

					if(!string.IsNullOrEmpty(sb)) {
						string[] sbbox = sb.Split(',');
						xll = float.Parse((string)sbbox[0]);
						yll = float.Parse((string)sbbox[1]); //, CultureInfo.InvariantCulture.NumberFormat);
						xur = float.Parse((string)sbbox[2]);
						yur = float.Parse((string)sbbox[3]);
						geoNewData["geolnm"] = "" + geoNewData["locality"] + "." + geoNewData["area_2"]; // TODO: create unique geo nm from cols locality, area_1, area_2 (props)
						geoNewData["geolx"] = (xll + xur) / 2; // FIXME: (i think these need to be in meters? check )
						geoNewData["geoly"] = (yll + yur) / 2; // FIXME:
						geoNewData["geolr"] = (yur - yll) * 60 * 1852/2; // FIXME: rough meters
						geoNewData["geolz"] = -1; //TODO: check/decide default
						long geo_id = geo.AddNew (alias, criterion, geoNewData, dbConn); // TODO: fix params
						entNewData["geolid"] = geo_id;				}
					else{
						xll = yll = xur = yur = 0;
					}
					//for( int j = 0; j < sbbox.Length; j++) sbbox[j] = sbbox[j].Trim(); 

					//double xll, yll, xur, yur;
					//xur = Double.TryParse((string)sbbox[2]); //, CultureInfo.InvariantCulture.NumberFormat);
					//yur = Double.TryParse((string)sbbox[3]); //, CultureInfo.InvariantCulture.NumberFormat);

					// TODO: Ent.Nd.Geo = if Geo regions exists within 80%, use existing Geo.id. Else, create new Geo and get Geo.id
					#endregion // geo

					#region ent pic: create doc
					// nm default = srcTYpeName+nm+profilepic
					// case "enttpic_docuid":
					// 1. add doc
					docNewData["docuconttyp"] = "picture";
					docNewData["docunm"] = "tw." + accNewData["acctsnm"] + ".profpic";
					long doc_id = doc.AddNew(alias, criterion, docNewData, dbConn);
					// 2. add ent doc id to ent newData
					entNewData["enttpic_docuid"] = doc_id;
					#endregion // ent pic: create doc

					#region lang_id
					long lang_id = 0;
					ArrayList critParams = new ArrayList {langNewData["lang"]};
					object[,] resultData = lang.search(alias, "idbynm", critParams, 0L, -1L, null);
					if(resultData.Length == 1){
						lang_id = (long)resultData[1,0]; //TODO: check
					}
					else if( resultData.Length == 0) {
						// if doesn't exist, add new lang
						lang_id = lang.AddNew(null, "default", langNewData, dbConn);
					}
					entNewData["enttlang_langid"] = lang_id;

					#endregion // lang_id

					#region ent
					long ent_id = ent.AddNew(alias, criterion, entNewData, dbConn);
					#endregion // ent

					#region account
					accNewData["acctentity_enttid"] = ent_id;
					accNewData["acctsource_srceid"] = 1L; // FIXME: get source id from db, currently hardcoded to tw id
					acc.AddNew(alias, criterion, accNewData, dbConn);
					#endregion //account


					#region edge
					edgeNewData["edgeprov_nodeid"] = ent_id;
					// TODO: 
					edgeNewData["edgetyp_edtpid"] = 10; 
					edgeNewData["edgesub_nodeid"] = edgeNewData["statusesCount"];
					edg.AddNew(alias, criterion,edgeNewData, dbConn);

					edgeNewData["edgetyp_edtpid"] = 12;
					edgeNewData["edgesub_nodeid"] = edgeNewData["favoritesCount"];
					edg.AddNew(alias, criterion,edgeNewData, dbConn);

					edgeNewData["edgetyp_edtpid"] = 13; 
					edgeNewData["edgesub_nodeid"] = edgeNewData["friendsCount"];
					edg.AddNew(alias, criterion,edgeNewData, dbConn);

					edgeNewData["edgetyp_edtpid"] = 11; 
					edgeNewData["edgesub_nodeid"] = edgeNewData["followersCount"];
					edg.AddNew(alias, criterion,edgeNewData, dbConn);

					edgeNewData["edgetyp_edtpid"] = 14; 
					edgeNewData["edgesub_nodeid"] = edgeNewData["listedCount"];
					edg.AddNew(alias, criterion,edgeNewData, dbConn);
					#endregion //edge

					#region eninentityinfo
					infoNewData["eninentity_enttid"] = ent_id;
					// url
					string url = (string)infoNewData["eninval"];
					if(url != "NA" && !string.IsNullOrEmpty(url)){
						infoNewData["enininfotype_intpid"] = 11; 
						info.AddNew(alias, criterion, infoNewData, dbConn);
					}
					// unverified address
					string rawadd = (string)infoNewData["rawaddress"];
					if(rawadd != "NA" && !string.IsNullOrEmpty(rawadd) ){
						infoNewData["enininfotype_intpid"] = 9; 
						infoNewData["eninval"] = infoNewData["rawaddress"];
						info.AddNew(alias, criterion, infoNewData, dbConn);
					}
					#endregion
				} catch (Exception ex) {
					Console.Write ("error parsing line: {0}, column: {1}, value {2}\n{3}", i, props[prop], line[prop], ex.Message);
					// TODO: need to rollback on record per record basis (problem, testing, we rollback all)
				}

			} // for lines
		}// main


	} // Import class


}



