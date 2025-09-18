import type MockAdapter from 'axios-mock-adapter';

export const workflowMockHandlers = (mock: MockAdapter) => {
  // 模擬 GET /api/v1/crm/customerInfo 端點
  mock.onGet('/api/v1/crm/customerInfo').reply(200, {
    accountNumber: '8840-9801245',
    idNumber: 'A123456796',
    name: '王明月',
    nationality: '中華民國',
    contactPhone: '0912345678',
    mobilePhone: '0912345678',
    birthDate: '1961-01-20',
    age: 64,
    identityType: '本國自然人',
    occupationCategory: '08 金融業',
    companyName: '玉山證券',
    companyPhone: '02-55081212 #123',
    householdAddress: '台北市中山區天祥路８６號１樓',
    companyAddress: '台北市中山區撫順街４１巷１３號５樓',
    mailingAddress: '台北市中山區天祥路８６號１樓',
    bankAccount: '(808) 0521979123456',
    investmentLimit: '100萬',
    emergencyContact: '林大名',
    emergencyContactPhone: '0912987345',
    tradingAgent: '無',
    legalRepresentative: '無',
  });

  // 模擬 GET /api/v1/crm/reviewItems 端點
  mock.onGet('/api/v1/crm/reviewItems').reply(200, {
    items: [
      { id: 'customerBehavior', title: '1. 顧客情形', type: 'checkbox', options: [ { id: 'isCrypto', text: '法人顧客為虛擬通貨/加密資產業者' }, { id: 'refuseProvideInfo', text: '拒絕提供所要求的資料或拒絕配合審查', alert: '婉拒' }, { id: 'threaten', text: '企圖像從業人員行賄或威脅…', alert: '婉拒' }, { id: 'fakeInfo', text: '故意以難以辨認或偽裝之手寫筆跡填寫申請表格…', alert: '婉拒' }, { id: 'blacklist', text: '黑名單或交易資金疑似與恐怖組織、恐怖活動或資恐有關聯者', alert: '婉拒' }, { id: 'indifferent', text: '對交易表達興趣，而不要求了解有關商品特性…' }, { id: 'overlyConcerned', text: '對防制洗錢及打擊資恐政策或控管表現出不尋常的關切' }, ], selected: [], remarks: ['※ 任一勾選[婉拒]項目，應婉拒開戶或業務往來'], disabled: false, },
      { id: 'defaultRecord', title: '2. 顧客是否有違約紀錄', type: 'radio', options: [ { id: 'yes_settled', text: '是，且已結案', alert: 'EDD' }, { id: 'yes_unsettled', text: '是，未結案', alert: '婉拒' }, { id: 'no', text: '無違約紀錄', isDefault: true }, ], selected: 'no', remarks: ['資料日期 2025/09/14－違約：N'], disabled: false, },
      { id: 'multipleAccounts', title: '3. 顧客是否於市場上開立多戶', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: ['資料日期 2025/09/14', '普通開戶：8、信用開戶/現股當沖開戶：2/0、期貨開戶：1'], disabled: false, },
      { id: 'negativeNews', title: '4. 顧客是否與金融相關之負面新聞或重大案件', type: 'radio', options: [ { id: 'yes', text: '是', isDefault: true, alert: 'EDD' }, { id: 'no', text: '否' }, ], selected: 'yes', remarks: [ '資料日期 2025/08/01', '相似度分數：100、負面新聞：True', '※ 請核對集保洗錢資料是否與顧客相關，並自行上傳相關附件。' ], disabled: false, },
      { id: 'foreignNationality', title: '5. 顧客是否具有外國國籍或其他國家稅務身分', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否' }, ], selected: 'no', remarks: ['CRS具其他國籍稅務身分：查無CRS'], disabled: false, subFields: [ { id: 'country', type: 'text', label: '國家', value: '', placeholder: '請輸入國家名稱', disabled: false, condition: { onValue: 'yes' } } ] },
      { id: 'highRiskCountry', title: '6. 顧客是否由高風險國家之金融機構、自高風險國家之客戶介紹或法人實質受益人來自高風險國家', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否' }, ], selected: 'no', remarks: [], disabled: false, },
      { id: 'ageOver65', title: '7. 顧客年齡已達65歲以上', type: 'radio', options: [ { id: 'yes', text: '是' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: ['顧客年齡：64歲'], disabled: false, subFields: [ { id: 'elderFormDate', type: 'date', label: '高齡表單完成日期', value: '', placeholder: '請選擇日期', disabled: false, condition: { onValue: 'yes' } } ] },
      { id: 'noGeographicLink', title: '8. 顧客留存地址是否「無地緣性」(僅適用臨櫃新開戶)', type: 'radio', options: [ { id: 'yes', text: '是' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: [], disabled: true, alert: '請檢附【A003顧客訪查紀錄表】' },
      { id: 'specialOccupation', title: '9. 顧客(含未成年之法定代理人)職業是否為「說明欄所述之項目」', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: [ '顧客職業類別：08 金融業', '※ 備註：職業項目為12、15、16、22~34、36~37...' ], disabled: false, },
      { id: 'multiAccountAgent', title: '10. 顧客是否有擔任買賣代理且代理多戶或顧客之買賣代理人是否代理多戶?', type: 'radio', options: [ { id: 'yes', text: '是' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: ['顧客無買賣代理人'], disabled: false, },
      { id: 'isPep', title: '11. 顧客是否為現任或曾任國內外政府或國際組織之重要政治性職務人士及其關係人?', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: [ '資料日期 2025/08/01', '相似度分數：100、PEP：False、RCA：否' ], disabled: false, },
      { id: 'corpRepMulti', title: '12. 法人之法定代理人是否為其他法人戶之法定代理人(五戶(含)以上)?', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: [], disabled: true, },
      { id: 'corpCredit', title: '13. 法人戶申請之交易額度與其財務狀況是否相當?', type: 'group', selected: null, remarks: [], disabled: true, subFields: [ { id: 'foundingDate', type: 'date', label: '成立日期', value: '', placeholder: '請選擇日期', disabled: true }, { id: 'capital', type: 'text', label: '資本額', value: '', placeholder: '請輸入金額', unit: '萬元', disabled: true }, ] },
      { id: 'beneficiaryMulti', title: '14. 法人實質受益人是否擔任多帳戶之實質受益人(五戶(含)以上)?', type: 'radio', options: [ { id: 'yes', text: '是', alert: 'EDD' }, { id: 'no', text: '否', isDefault: true }, ], selected: 'no', remarks: [], disabled: true, },
    ],
    notes: '',
    requiresEDD: null,
  });

  // 模擬 POST /api/v1/crm/case 端點
  mock.onPost('/api/v1/crm/case').reply(config => {
    console.log('📠 Mock API (CRM) 收到 POST 請求:', JSON.parse(config.data).action);
    return [200, { status: 'success' }];
  });
};