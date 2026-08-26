# HSK 314 Reading Bank Design

Muc tieu:
- Luu duoc mot ngan hang chung gom HSK4 va HSK5
- Bo phan `Listening` va `Writing`
- Random de theo ty le 50/50 giua HSK4 va HSK5 ma khong vo cau truc doan doc / bo tu chung

## Van de cua schema hien tai

Bang hien tai `dt314_HskQuestions` + `dt314_HskAnswers` phu hop voi cau don, nhung chua du cho:
- 1 doan van dung cho nhieu cau
- 1 bo tu dung chung cho 5 cau
- random theo nhom thay vi tung cau

Neu random tung cau rieng:
- HSK4 cau 46-50, 51-55 se bi hong vi 5 cau dung chung 1 bo tu
- HSK4/HSK5 cau doc hieu se mat doan van chung

## Cau truc de can luu

HSK4 reading:
- `ReadingPart1`: cau 46-55, `SharedWordBank`
- `ReadingPart2`: cau 56-65, `SentenceOrder`
- `ReadingPart3`: cau 66-85, `SharedPassage` va `SingleQuestion`

HSK5 reading:
- `ReadingPart1`: cau 46-60, `PassageCloze`
- `ReadingPart2`: cau 61-70, `SingleQuestion`
- `ReadingPart3`: cau 71-90, `SharedPassage`

## Bang moi

### `dt314_HskSourcePaper`
Bang nay la optional. Neu khong quan tam ten de goc thi co the bo trong, khong can dung trong luong van hanh.

### `dt314_HskQuestionGroup`
Don vi random chinh.

Truong chinh:
- `LevelCode`
- `SectionCode`
- `PartCode`
- `GroupType`
- `InstructionText`
- `SharedPassage`
- `SharedOptionPool`
- `SourceQuestionFrom`
- `SourceQuestionTo`
- `QuestionCount`
- `RandomAsUnit`

### Mo rong `dt314_HskQuestions`
- `GroupId`
- `SourcePaperId`
- `PartCode`
- `SourceQuestionNo`
- `SourceQuestionSubNo`
- `QuestionCode`
- `TopicTag`
- `DifficultyWeight`
- `UsageCount`
- `LastUsedDate`

### Mo rong `dt314_HskExamQuestion`
- `GroupId`
- `GroupDisplayOrder`
- `QuestionOrderInGroup`

## Cach import

1. Tao `QuestionGroup`
2. Tao cac `Question` nam trong group
3. Tao `Answer` cho tung question
4. Gan `LevelCode = HSK4 / HSK5`

Quy uoc import:
- 1 doan doc chung = 1 group
- 1 bo tu chung = 1 group
- cau doc le khong co doan chung co the la `SingleQuestion`

## Cach random

Khong random theo `Question` truoc.
Phai random theo `QuestionGroup`, sau do moi mo rong ra danh sach question hien thi.

Goi y blueprint:
- `HSK4`: 10-20 cau
- `HSK5`: 10-20 cau
- co the chia tiep theo `PartCode` de de can bang

Vi du:
- 1 group `SharedWordBank` HSK4 = 5 cau
- 1 group `SentenceOrder` HSK4 = 1 cau
- 1 group doan doc HSK5 = 3-4 cau

## Cach su dung voi code hien tai

Code hien tai van chay vi cac cot moi deu nullable.

Khi nang cap builder:
1. lay pool tu `dt314_HskQuestionGroup`
2. chia target theo so cau: `50% HSK4`, `50% HSK5`
3. random group theo level + part cho den khi du so cau
4. lay danh sach question thuoc group
5. luu snapshot vao `dt314_HskExamQuestion`
6. khi render bai thi, hien `SharedPassage` / `SharedOptionPool` cua group

## Goi y buoc tiep theo

Trang thai hien tai:
- da co script mo rong DB
- da co code random theo group cho Reading
- da co import Excel theo `GroupNo / GroupType / PartCode`

Viec nen lam tiep:
- viet script seed that tu 4 file PDF da co
- neu muon UI dep hon, gom hien thi theo block group thay vi lap lai passage o moi cau
