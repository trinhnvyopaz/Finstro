using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Finstro.Serverless.Models.Entity;
using Finstro.Serverless.Models.Response;

namespace Finstro.Serverless.Dapper.Repository
{
    public sealed class ClientRepository : Repository<ClientEntity>
    {
        public ClientRepository() : base("client", "party_id") { }        public ClientRepository(string tableName) : base(tableName) { }        public ClientRepository(IDatabaseConnectionFactory connectionFactory, string tableName) : base(connectionFactory, tableName) { }

        public IEnumerable<ClientListResponse> GetClientList(string search, string[] fields)
        {
            object param = new
            {
                Text = search
            };

            return Get<ClientListResponse>($@"
                             SELECT DISTINCT b.party_id business_party_id, 
		                            p.modified_on,
                                    b.abn,
                                    CASE IFNULL(ab.company_name, '')
			                            WHEN '' THEN ab.company_legal_name
			                            ELSE ab.company_name
		                            END as company_name,
                                    CONCAT_WS(' ', pe.first_name, pe.last_name) as director_name,
                                    CASE cl.Status
			                            WHEN 0 THEN 'INACTIVE'
			                            WHEN 1 THEN 'ACTIVE'
			                            WHEN 2 THEN 'SUSPENDED'
			                            WHEN 3 THEN 'PENDING'
			                            WHEN 4 THEN 'DECLINED'
			                            ELSE '-'
		                            END as client_status,
                                    ca.type as product_type,
                                    ca.facility_limit
                               FROM party p
                              INNER JOIN business b
                                 ON p.party_id = b.party_id
	                            AND b.abn is not null
                                AND b.company_name is not null
                              INNER JOIN asic_business ab
                                 ON b.abn = ab.abn
	                            AND b.company_name = ab.company_name
                              INNER JOIN business_person bp
                                 ON b.party_id = bp.business_party_id
	                            AND bp.relation_type = 'DIRECTOR'
                                AND bp.business_person_id = (SELECT bp2.business_person_id 
								                               FROM business_person bp2 
								                              WHERE bp2.business_party_id = bp.business_party_id 
                                                              ORDER BY bp2.business_person_id LIMIT 1)
                              INNER JOIN person pe
                                 ON bp.person_party_id = pe.party_id
                               LEFT JOIN client cl
	                             ON cl.party_id = p.party_id
                            LEFT JOIN client_account ca
	                             ON ca.party_id = p.party_id
                              WHERE CONCAT({string.Join(", ", fields)})  LIKE CONCAT('%', @Text ,'%')
                              ORDER BY b.party_id DESC", param);
        }

        public dynamic GetCreditApplicationSummary(int businessPartyId)
        {
            object param = new
            {
                BusinessPartyId = businessPartyId
            };

            return Get<dynamic>($@"
                                 SELECT DISTINCT b.party_id BusinessPartyId, 
		                                p.modified_on as DateJoined,
		                                CASE IFNULL(ab.company_name, '')
			                                WHEN '' THEN ab.company_legal_name
			                                ELSE ab.company_name
		                                END as CompanyName,
		                                IFNULL(ca.facility_limit, 0) as 'Limit',
		                                0.00 Balance,
		                                0.00 Overdue,
		                                0.00 Available,
                                        ab.abn,
		                                ab.acn,
		                                ab.company_legal_name,
		                                ab.company_name,
		                                a.address_id,
		                                a.country,
		                                a.post_code,
		                                a.state,
		                                a.street_name,
		                                a.street_number,
		                                a.street_type,
		                                a.suburb,
		                                a.unit_or_level
                                   FROM party p
                                  INNER JOIN business b
	                                 ON p.party_id = b.party_id
                                  INNER JOIN asic_business ab
	                                 ON b.abn = ab.abn
	                                AND b.company_name = ab.company_name     
                                   LEFT JOIN address a
	                                 ON p.address_id = a.address_id
                                   LEFT JOIN client_account ca
	                                 ON ca.party_id = p.party_id     
                                  WHERE b.party_id = @BusinessPartyId ", param).FirstOrDefault();
        }

        public dynamic GetCreditApplicationFirstContact(int businessPartyId)
        {
            object param = new
            {
                BusinessPartyId = businessPartyId
            };

            return Get<dynamic>($@"
                                 SELECT pe.email as first_contact_email,
                                        pe.last_name as first_contact_last_name,
                                        pe.first_name as first_contact_first_name,
                                        pe.mobile as first_contact_mobile,
                                        pe.party_id as first_contact_party_id,
                                        '' as first_contact_uuid,
                                        0 as first_contact_accepted
                                   FROM business_person bp
                                  INNER JOIN person pe
                                     ON bp.person_party_id = pe.party_id
                                  WHERE business_party_id = @BusinessPartyId ", param).FirstOrDefault();
        }


        public dynamic GetCreditApplicationMedicare(int personPartyId)
        {
            object param = new
            {
                PersonPartyId = personPartyId
            };

            return Get<dynamic>($@"
                                 SELECT m.card_color as medicare_card_color,
                                        m.card_number as medicare_card_number,
                                        m.card_number_ref as medicare_card_number_ref,
                                        m.valid_to as medicare_valid_to,
                                        m.identification_id as medicare_identification_id
                                   FROM person pe
                                  INNER JOIN identification idt
                                     ON idt.party_id = pe.party_id
                                    AND idt.identification_id = (SELECT MAX(m.identification_id)
                                                                FROM identification i
                                                                INNER JOIN medicare m
                                                                    ON i.identification_id = m.identification_id
                                                                WHERE party_id = pe.party_id)
                                  INNER JOIN medicare m
                                     ON idt.identification_id = m.identification_id
                                  WHERE pe.party_id = @PersonPartyId ", param).FirstOrDefault();
        }



        public dynamic GetCreditApplicationDriversLicense(int personPartyId)
        {
            object param = new
            {
                PersonPartyId = personPartyId
            };

            return Get<dynamic>($@"
                                 SELECT dl.card_number as driver_licence_card_number,
                                        dl.date_of_birth as driver_licence_date_of_birth,
                                        dl.licence_number as driver_licence_licence_number,
                                        dl.state as driver_licence_state,
                                        dl.valid_to as driver_licence_valid_to,
                                        dl.identification_id as driver_licence_identification_id
                                    FROM person pe
                                    INNER JOIN identification idt
                                        ON idt.party_id = pe.party_id
                                    AND idt.identification_id = ( SELECT MAX(dr.identification_id)
								                                    FROM identification i
								                                INNER JOIN driver_licence dr
								                                    ON i.identification_id = dr.identification_id
								                                WHERE party_id = pe.party_id)  

                                    LEFT JOIN driver_licence dl 
                                        ON idt.identification_id = dl.identification_id
                                    WHERE pe.party_id = @PersonPartyId ", param).FirstOrDefault();
        }

        public dynamic GetCreditApplicationBusinessAddress(int businessPartyId)
        {
            object param = new
            {
                BusinessPartyId = businessPartyId
            };

            return Get<dynamic>($@"
                                 SELECT a.address_id as residential_address_id,
		                                a.country as residential_country,
		                                a.post_code as residential_post_code,
		                                a.state as residential_state,
		                                a.street_name as residential_street_name,
		                                a.street_number as residential_street_number,
		                                a.street_type as residential_street_type,
		                                a.suburb as residential_suburb
                                   FROM party p
                                  INNER JOIN business b
	                                 ON p.party_id = b.party_id
                                   LEFT JOIN address a
	                                 ON p.address_id = a.address_id
                                  WHERE p.party_id = @BusinessPartyId", param).FirstOrDefault();
        }


        public dynamic GetCreditApplicationHeader(int businessPartyId)
        {
            object param = new
            {
                BusinessPartyId = businessPartyId
            };

            return Get<dynamic>($@"
                                 SELECT DISTINCT b.party_id BusinessPartyId, 
		                                CASE IFNULL(ab.company_name, '')
			                                WHEN '' THEN ab.company_legal_name
			                                ELSE ab.company_name
		                                END as CompanyName,
	                                    CASE WHEN ab.acn IS NULL or ab.acn = ''
                                            THEN ab.abn
                                            ELSE ab.acn
                                        END as AbnAcn,    
                                        a.post_code,
		                                IFNULL(ca.facility_limit, 0) as RequestedLimit,
		                                0.00 CurrentLimit,
		                                0.00 CurrentBalance,
                                        pe.email,
                                        pe.mobile,
                                        '' as WebSite,
                                        null as IncorporationDate,
    	                                (SELECT MAX(Value)
		                                   FROM assessment_input ai
		                                  INNER JOIN assessment_input_attribute aa
			                                 ON ai.assessment_input_id = aa.assessment_input_id
		                                  WHERE ab.type = ai.type
			                                AND ai.party_id = b.party_id 
			                                AND attribute = 'AgeOfFile') as TimeTrading,
		                                (SELECT MAX(Value)
		                                   FROM assessment_input ai
		                                  INNER JOIN assessment_input_attribute aa
			                                 ON ai.assessment_input_id = aa.assessment_input_id
		                                  WHERE ab.type = ai.type
			                                AND ai.party_id = b.party_id 
			                                AND attribute = 'AgeOfABNRegisteredForGST') as GSTDate
                                   FROM party p
                                  INNER JOIN business b
	                                 ON p.party_id = b.party_id
                                 INNER JOIN business_person bp
	                                 ON b.party_id = bp.business_party_id
                                    AND bp.person_party_id = (SELECT MIN(person_party_id) 
								                                FROM business_person 
							                                   WHERE business_party_id = bp.business_party_id)
                                  INNER JOIN person pe
                                     ON bp.person_party_id = pe.party_id
                                  INNER JOIN asic_business ab
	                                 ON b.abn = ab.abn
	                                AND b.company_name = ab.company_name     
                                   LEFT JOIN client_account ca
	                                 ON ca.party_id = p.party_id
                                   LEFT JOIN address a
	                                 ON p.address_id = a.address_id
                                  WHERE b.party_id = @BusinessPartyId", param).FirstOrDefault();
        }



        public dynamic GetCreditApplicationDetail(int creditApplicationId)
        {
            object param = new
            {
                BusinessPartyId = creditApplicationId
            };

            return Get<dynamic>($@"
                                 SELECT DISTINCT b.party_id BusinessPartyId, 
		                                p.modified_on as DateJoined,
		                                CASE IFNULL(ab.company_name, '')
			                                WHEN '' THEN ab.company_legal_name
			                                ELSE ab.company_name
		                                END as CompanyName,
		                                IFNULL(ca.facility_limit, 0) as 'Limit',
		                                0.00 Balance,
		                                0.00 Overdue,
		                                0.00 Available,
		                                ab.abn,
		                                ab.acn,
		                                ab.company_legal_name,
		                                ab.company_name,
		                                (SELECT IFNULL(value,0) 
		                                   FROM party_attribute 
		                                  WHERE attribute = 3  /*Attribute value 3 means 'SELECTED_CREDIT_AMOUNT'*/
			                                AND party_id = b.party_id) as SelectedCreditAmount,
		                                CASE ab.type
			                                 WHEN 0 THEN 'COMPANY'
			                                 WHEN 1 THEN 'INDIVIDUAL'
			                                 WHEN 2 THEN 'BUSINESS'
			                                 WHEN 3 THEN 'COMPANY'
			                                 WHEN 4 THEN 'BUSINESS'
			                                 ELSE ''
		                                END as BusinessType,
		                                (SELECT Value  
		                                   FROM assessment_input ai
		                                  INNER JOIN assessment_input_attribute aa
			                                 ON ai.assessment_input_id = aa.assessment_input_id
		                                  WHERE ab.type = ai.type
			                                AND ai.party_id = b.party_id 
			                                AND attribute = 'AgeOfFile') TimeTrading,
		                                CASE ab.type
			                                 WHEN 0 THEN 100
			                                 WHEN 1 THEN 1
			                                 WHEN 2 THEN 100
			                                 WHEN 3 THEN 100
			                                 WHEN 4 THEN 100
			                                 ELSE ''
		                                END as DirectorsCount,
		                                a.address_id,
		                                a.country,
		                                a.post_code,
		                                a.state,
		                                a.street_name,
		                                a.street_number,
		                                a.street_type,
		                                a.suburb,
		                                a.unit_or_level,
                                        pe.email as first_contact_email,
                                        pe.last_name as first_contact_last_name,
                                        pe.first_name as first_contact_first_name,
                                        pe.mobile as first_contact_mobile,
                                        pe.party_id as first_contact_party_id,
                                        '' as first_contact_uuid,
                                        0 as first_contact_accepted,
                                        m.card_color as medicare_card_color,
                                        m.card_number as medicare_card_number,
                                        m.card_number_ref as medicare_card_number_ref,
                                        m.valid_to as medicare_valid_to,
		                                m.identification_id as medicare_identification_id,
                                        ap.address_id as residential_address_id,
		                                ap.country as residential_country,
		                                ap.post_code as residential_post_code,
		                                ap.state as residential_state,
		                                ap.street_name as residential_street_name,
		                                ap.street_number as residential_street_number,
		                                ap.street_type as residential_street_type,
		                                ap.suburb as residential_suburb,
		                                ap.unit_or_level as residential_unit_or_level,
                                        dl.card_number as driver_licence_card_number,
                                        dl.date_of_birth as driver_licence_date_of_birth,
                                        dl.licence_number as driver_licence_licence_number,
                                        dl.state as driver_licence_state,
                                        dl.valid_to as driver_licence_valid_to,
                                        dl.identification_id as driver_licence_identification_id

                                   FROM party p
                                  INNER JOIN business b
	                                 ON p.party_id = b.party_id
	                                AND b.abn is not null
	                                AND b.company_name is not null
                                  INNER JOIN asic_business ab
	                                 ON b.abn = ab.abn
	                                AND b.company_name = ab.company_name
                                   LEFT JOIN client_account ca
	                                 ON ca.party_id = p.party_id
                                   LEFT JOIN address a
	                                 ON b.party_id = a.address_id
                                  INNER JOIN business_person bp
	                                 ON b.party_id = bp.business_party_id
	                                AND bp.relation_type = 'DIRECTOR'
	                                AND bp.person_party_id = (SELECT bp2.person_party_id 
								                                   FROM business_person bp2 
								                                  WHERE bp2.business_party_id = bp.business_party_id 
								                                  ORDER BY bp2.business_person_id LIMIT 1)   
                                  INNER JOIN person pe
	                                 ON bp.person_party_id = pe.party_id         
                                   LEFT JOIN identification idt
                                     ON idt.party_id = pe.party_id
                                   AND idt.identification_id = ( SELECT MAX(m.identification_id)
								                                 FROM identification i
								                                INNER JOIN medicare m
								                                   ON i.identification_id = m.identification_id
								                                WHERE party_id = pe.party_id)  
                                   LEFT JOIN address ap
                                     ON ap.address_id = (SELECT address_id FROM party WHERE party_id = bp.person_party_id LIMIT 1)    
                                   LEFT JOIN medicare m 
                                     ON idt.identification_id = m.identification_id
                                   LEFT JOIN identification idd
                                     ON idd.party_id = pe.party_id
                                    AND idd.identification_id = ( SELECT MAX(dr.identification_id)
								                                 FROM identification i
								                                INNER JOIN driver_licence dr
								                                   ON i.identification_id = dr.identification_id
								                                WHERE party_id = pe.party_id)  
                                   LEFT JOIN driver_licence dl 
                                     ON idd.identification_id = dl.identification_id

                                  WHERE b.party_id = @BusinessPartyId ", param).FirstOrDefault();
        }
    }


}
